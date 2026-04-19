using GSS.Core.Runtime.Enums;
using GSS.Core.Runtime.Exceptions;
using GSS.Core.Runtime.Memory;

namespace GSS.Core.Runtime.Execution
{
	public sealed class RuntimeScheduler
	{
		private readonly ExecutionContextPool _pool;
		private readonly List<RuntimeExecutionContext> _activeContexts;

		public int MaxStepsPerTick { get; set; } = 10000;
		public Action<Exception, RuntimeExecutionContext>? OnScriptFaulted { get; set; }

		public RuntimeScheduler(ExecutionContextPool pool, int initialCapacity = 256)
		{
			_pool = pool;
			_activeContexts = new List<RuntimeExecutionContext>(initialCapacity);
		}

		public void Enqueue(object instance, ExecutableGraph graph)
		{
			var context = _pool.Rent();
			context.Initialize(instance, graph);
			_activeContexts.Add(context);
		}

		public void Tick(float deltaTime)
		{
			for (int i = _activeContexts.Count - 1; i >= 0; i--)
			{
				var context = _activeContexts[i];
				context.StateTimer += deltaTime;

				try
				{
					var state = RunContext(context);

					if (state != ExecutionState.Running)
					{
						RemoveContextAt(i);
					}
				}
				catch (Exception ex)
				{
					OnScriptFaulted?.Invoke(ex, context);
					RemoveContextAt(i);
				}
			}
		}

		private ExecutionState RunContext(RuntimeExecutionContext context)
		{
			var instructions = context.Graph.Instructions;
			int stepCount = 0;

			while (context.InstructionPointer >= 0 && context.InstructionPointer < instructions.Length)
			{
				if (stepCount++ >= MaxStepsPerTick)
				{
					throw new GssInfiniteLoopException(context.InstructionPointer, MaxStepsPerTick);
				}

				var currentInstruction = instructions[context.InstructionPointer];
				var state = currentInstruction.Execute(context);

				if (context.InstructionPointer < 0 || context.InstructionPointer > instructions.Length)
				{
					throw new GssInstructionOutOfBoundsException(context.InstructionPointer, instructions.Length);
				}

				if (state == ExecutionState.Running)
				{
					return ExecutionState.Running;
				}

				if (state == ExecutionState.Failure || state == ExecutionState.Halted)
				{
					return state;
				}
			}

			return ExecutionState.Success;
		}

		private void RemoveContextAt(int index)
		{
			var context = _activeContexts[index];
			_pool.Return(context);

			int lastIndex = _activeContexts.Count - 1;

			if (index != lastIndex)
			{
				_activeContexts[index] = _activeContexts[lastIndex];
			}

			_activeContexts.RemoveAt(lastIndex);
		}
	}
}