using FluentAssertions;
using Gss.Core.Tests.ApiBinding;
using GSS.Core.ApiBinding.Models;
using GSS.Core.ApiBinding.Registry;
using GSS.Core.Runtime.Exceptions;
using GSS.Core.Runtime.Execution;
using GSS.Core.Runtime.Instructions.Abstractions;
using GSS.Core.Runtime.Instructions.Async;
using GSS.Core.Runtime.Instructions.Core;
using GSS.Core.Runtime.Instructions.Interop;
using GSS.Core.Runtime.Instructions.Memory;
using GSS.Core.Runtime.Instructions.System;
using GSS.Core.Runtime.Instructions.Types;
using GSS.Core.Runtime.Memory;

namespace Gss.Core.Tests.Runtime
{
	[TestFixture]
	public class RuntimeEngineTests
	{
		private ExecutionContextPool _pool = null!;
		private RuntimeScheduler _scheduler = null!;
		private ApiRegistry _registry = null!;
		private Character _targetInstance = null!;

		[SetUp]
		public void Setup()
		{
			_pool = new ExecutionContextPool();
			_scheduler = new RuntimeScheduler(_pool) { MaxStepsPerTick = 1000 };
			_targetInstance = new Character();

			var builder = new ApiRegistryBuilder();
			builder.Register(new CharacterApiProfile());
			_registry = (ApiRegistry)builder.Build();
		}

		[Test]
		public void ExecutionContext_Initialize_ShouldAllocateCorrectBufferSizes()
		{
			var context = _pool.Rent();
			var graph = new ExecutableGraph(Array.Empty<IInstruction>(), variableCount: 5, maxArgumentCount: 3);

			context.Initialize(_targetInstance, graph);

			context.GetArgBuffer(3).Length.Should().Be(3);
			context.SetRegister(4, GssValue.FromInt(10));
			context.GetRegister(4).AsInt.Should().Be(10);
		}

		[Test]
		public void ExecutionContext_Reset_ShouldClearAllData()
		{
			var context = _pool.Rent();
			var graph = new ExecutableGraph(Array.Empty<IInstruction>(), variableCount: 2, maxArgumentCount: 2);
			context.Initialize(_targetInstance, graph);

			context.SetRegister(0, GssValue.FromInt(99));
			context.Accumulator = GssValue.FromFloat(1f);
			context.InstructionPointer = 5;

			context.Reset();

			context.InstructionPointer.Should().Be(0);
			context.Accumulator.Type.Should().Be(GssType.Null);
			context.TargetInstance.Should().BeNull();
			context.Graph.Should().BeNull();
		}

		[Test]
		public void Instruction_LoadAndStore_ShouldMoveDataCorrectly()
		{
			var context = new RuntimeExecutionContext();
			context.Initialize(_targetInstance, new ExecutableGraph(Array.Empty<IInstruction>(), 1, 0));

			var loadConst = new LoadConstantInstruction(GssValue.FromInt(42));
			var store = new StoreInstruction(0);
			var load = new LoadInstruction(0);

			loadConst.Execute(context);
			context.Accumulator.AsInt.Should().Be(42);

			store.Execute(context);
			context.Accumulator = GssValue.Null; // Simulate accumulator change

			load.Execute(context);
			context.Accumulator.AsInt.Should().Be(42);
		}

		[Test]
		public void Instruction_Branch_WhenTrue_ShouldJumpToCorrectPointer()
		{
			var context = new RuntimeExecutionContext();
			context.Initialize(_targetInstance, new ExecutableGraph(Array.Empty<IInstruction>(), 0, 0));
			context.Accumulator = GssValue.FromBool(true);

			var branch = new BranchInstruction(trueJumpIndex: 10, falseJumpIndex: 20);
			branch.Execute(context);

			context.InstructionPointer.Should().Be(10);
		}

		[Test]
		public void Instruction_Arithmetic_DivFloat_ByZero_ShouldThrowException()
		{
			var context = new RuntimeExecutionContext();
			context.Initialize(_targetInstance, new ExecutableGraph(Array.Empty<IInstruction>(), 1, 0));
			context.Accumulator = GssValue.FromFloat(10f);
			context.SetRegister(0, GssValue.FromFloat(0f));

			var div = new DivFloatInstruction(0);

			Action act = () => div.Execute(context);

			act.Should().Throw<GssDivideByZeroException>();
		}

		[Test]
		public void Instruction_Logic_AndBool_ShouldCalculateCorrectly()
		{
			var context = new RuntimeExecutionContext();
			context.Initialize(_targetInstance, new ExecutableGraph(Array.Empty<IInstruction>(), 1, 0));
			context.Accumulator = GssValue.FromBool(true);
			context.SetRegister(0, GssValue.FromBool(false));

			var and = new AndBoolInstruction(0);
			and.Execute(context);

			context.Accumulator.AsBool.Should().BeFalse();
		}

		[Test]
		public void Instruction_CallMethod_ShouldInvokeTargetApi()
		{
			var classMeta = _registry.RetrieveClass("Character");
			var healMethod = classMeta.RetrieveMethod("Heal");
			_targetInstance.Health = 50f;

			var context = new RuntimeExecutionContext();
			context.Initialize(_targetInstance, new ExecutableGraph(Array.Empty<IInstruction>(), 1, 1));

			// Setup argument
			context.SetRegister(0, GssValue.FromFloat(20f));

			var call = new CallMethodInstruction(healMethod, new[] { 0 });
			call.Execute(context);

			_targetInstance.Health.Should().Be(70f);
		}

		[Test]
		public void Scheduler_GraphExecution_ShouldCompleteSynchronously()
		{
			var instructions = new IInstruction[]
			{
				new LoadConstantInstruction(GssValue.FromInt(5)),
				new StoreInstruction(0),
				new LoadConstantInstruction(GssValue.FromInt(10)),
				new AddIntInstruction(0)
			};

			var graph = new ExecutableGraph(instructions, 1, 0);
			_scheduler.Enqueue(_targetInstance, graph);

			_scheduler.Tick(0.1f);

			// Ensure nothing faulted
			Exception? fault = null;
			_scheduler.OnScriptFaulted = (ex, ctx) => fault = ex;

			fault.Should().BeNull();
		}

		[Test]
		public void Scheduler_WaitInstruction_ShouldSuspendAndResumeOnTick()
		{
			var classMeta = _registry.RetrieveClass("Character");
			var healMethod = classMeta.RetrieveMethod("Heal");
			_targetInstance.Health = 50f;

			var instructions = new IInstruction[]
			{
				new WaitInstruction(2.0f),
				new LoadConstantInstruction(GssValue.FromFloat(15f)),
				new StoreInstruction(0),
				new CallMethodInstruction(healMethod, new[] { 0 })
			};

			var graph = new ExecutableGraph(instructions, 1, 1);
			_scheduler.Enqueue(_targetInstance, graph);

			_scheduler.Tick(1.0f); // Time = 1.0
			_targetInstance.Health.Should().Be(50f); // Not healed yet

			_scheduler.Tick(1.5f); // Time = 2.5 (Surpasses 2.0 limit)
			_targetInstance.Health.Should().Be(65f); // Resumed and healed
		}

		[Test]
		public void Scheduler_InfiniteLoop_ShouldThrowAndTriggerFaultCallback()
		{
			Exception? faultEx = null;
			_scheduler.OnScriptFaulted = (ex, ctx) => faultEx = ex;

			var instructions = new IInstruction[]
			{
				new JumpInstruction(0) // Jumps to itself continually
			};

			var graph = new ExecutableGraph(instructions, 0, 0);
			_scheduler.Enqueue(_targetInstance, graph);

			_scheduler.Tick(1.0f);

			faultEx.Should().NotBeNull();
			faultEx.Should().BeOfType<GssInfiniteLoopException>();
		}

		[Test]
		public void Scheduler_OutOfBoundsInstruction_ShouldTriggerFaultCallback()
		{
			Exception? faultEx = null;
			_scheduler.OnScriptFaulted = (ex, ctx) => faultEx = ex;

			var instructions = new IInstruction[]
			{
				new JumpInstruction(50) // Jumps outside array bounds
			};

			var graph = new ExecutableGraph(instructions, 0, 0);
			_scheduler.Enqueue(_targetInstance, graph);

			_scheduler.Tick(1.0f);

			faultEx.Should().NotBeNull();
			faultEx.Should().BeOfType<GssInstructionOutOfBoundsException>();
		}

		[Test]
		public void Scheduler_AbortInstruction_ShouldHaltAndTriggerFaultCallback()
		{
			Exception? faultEx = null;
			_scheduler.OnScriptFaulted = (ex, ctx) => faultEx = ex;

			var instructions = new IInstruction[]
			{
				new AbortInstruction("Critical failure test")
			};

			var graph = new ExecutableGraph(instructions, 0, 0);
			_scheduler.Enqueue(_targetInstance, graph);

			_scheduler.Tick(1.0f);

			faultEx.Should().NotBeNull();
			faultEx.Should().BeOfType<GssRuntimeException>();
			faultEx!.Message.Should().Contain("Critical failure test");
		}
	}
}