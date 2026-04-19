using System.Diagnostics;
using FluentAssertions;
using GSS.Core.ApiBinding.Abstractions;
using GSS.Core.ApiBinding.Models;
using GSS.Core.ApiBinding.Registry;

namespace Gss.Core.Tests.ApiBinding
{
	[TestFixture]
	[Category("Performance")]
	public class ApiBindingPerformanceTests
	{
		private IClassMetadata _meta;
		private Character _character;
		private const int Iterations = 200_000;

		[OneTimeSetUp]
		public void ZeroLeakSetup()
		{
			var builder = new ApiRegistryBuilder();
			builder.Register(new CharacterApiProfile());
			var registry = builder.Build();
			_meta = registry.RetrieveClass("Character");
			_character = new Character();

			// Прогрів JIT (Warm-up)
			for (int i = 0; i < 10000; i++)
			{
				RunDirect();
				RunSystem();
			}
		}

		[Test]
		public void StressTest_Property_Access_Performance()
		{
			var propMeta = _meta.RetrieveProperty("Level");
			var newValue = GssValue.FromInt(50);

			// 1. Direct Access
			var swDirect = Stopwatch.StartNew();
			for (int i = 0; i < Iterations; i++)
			{
				_character.Level = 50;
				int val = _character.Level;
			}
			swDirect.Stop();

			// 2. System Access (with Validation)
			var swSystem = Stopwatch.StartNew();
			for (int i = 0; i < Iterations; i++)
			{
				propMeta.SetValue(_character, newValue);
				var val = propMeta.GetValue(_character);
			}
			swSystem.Stop();

			Console.WriteLine($"[Properties] Direct: {swDirect.ElapsedMilliseconds}ms | System: {swSystem.ElapsedMilliseconds}ms");
			Console.WriteLine($"[Properties] Overhead per call: ~{(double)(swSystem.Elapsed.TotalMilliseconds - swDirect.Elapsed.TotalMilliseconds) / Iterations * 1000000:F2} ns");

			// Assert
			_character.Level.Should().Be(50);
		}

		[Test]
		public void StressTest_Method_Arity_1_Performance()
		{
			var methodMeta = _meta.RetrieveMethod("Heal");
			var healValue = GssValue.FromFloat(1.0f);
			var args = new[] { healValue };

			// 1. Direct Access
			var swDirect = Stopwatch.StartNew();
			for (int i = 0; i < Iterations; i++)
			{
				_character.ApplyHeal(1.0f);
			}
			swDirect.Stop();

			// 2. System Access
			var swSystem = Stopwatch.StartNew();
			for (int i = 0; i < Iterations; i++)
			{
				methodMeta.Invoke(_character, args);
			}
			swSystem.Stop();

			Console.WriteLine($"[Method Arity 1] Direct: {swDirect.ElapsedMilliseconds}ms | System: {swSystem.ElapsedMilliseconds}ms");

			// Assert
			_character.Health.Should().BeGreaterThan(0);
		}

		[Test]
		public void StressTest_Method_Arity_5_Performance()
		{
			var methodMeta = _meta.RetrieveMethod("Ultimate");
			var args = new[] {
				GssValue.FromInt(10),
				GssValue.FromFloat(1.1f),
				GssValue.FromBool(false),
				GssValue.FromObject("vfx"),
				GssValue.FromObject(ElementType.Fire)
			};

			// 1. Direct Access
			var swDirect = Stopwatch.StartNew();
			for (int i = 0; i < Iterations; i++)
			{
				_character.ExecuteUltimate(10, 1.1f, false, "vfx", ElementType.Fire);
			}
			swDirect.Stop();

			// 2. System Access (Unboxing 5 args + Validation)
			var swSystem = Stopwatch.StartNew();
			for (int i = 0; i < Iterations; i++)
			{
				methodMeta.Invoke(_character, args);
			}
			swSystem.Stop();

			Console.WriteLine($"[Method Arity 5] Direct: {swDirect.ElapsedMilliseconds}ms | System: {swSystem.ElapsedMilliseconds}ms");

			// Assert
			_character.Health.Should().BeGreaterThan(0);
		}

		private void RunDirect()
		{
			_character.Level = 10;
			_character.ApplyHeal(5f);
			_character.ExecuteUltimate(5, 1f, false, "", ElementType.Water);
		}

		private void RunSystem()
		{
			var p = _meta.RetrieveProperty("Level");
			p.SetValue(_character, GssValue.FromInt(10));

			var m1 = _meta.RetrieveMethod("Heal");
			m1.Invoke(_character, new[] { GssValue.FromFloat(5f) });
		}
	}
}