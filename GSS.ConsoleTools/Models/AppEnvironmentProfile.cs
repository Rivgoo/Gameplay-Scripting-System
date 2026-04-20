using GSS.Core.ApiBinding.Abstractions;
using GSS.Core.ApiBinding.Builders;

namespace GSS.ConsoleApp.Models
{
	public sealed class AppEnvironmentProfile : ApiProfile<AppEnvironment>
	{
		public override void Build(IApiBindingScope<AppEnvironment> scope)
		{
			scope.Property<string>("PlayerName")
				.Get(e => e.PlayerName)
				.Set((e, v) => e.PlayerName = v);

			scope.Property<float>("PlayerHealth")
				.Get(e => e.PlayerHealth)
				.Set((e, v) => e.PlayerHealth = v);

			scope.Method("Print")
				.Param<string>("message")
				.Handler((e, m) => e.Print(m));

			scope.Method("PrintInt")
				.Param<int>("value")
				.Handler((e, v) => e.PrintInt(v));

			scope.Method("PrintFloat")
				.Param<float>("value")
				.Handler((e, v) => e.PrintFloat(v));

			scope.Method("PrintBool")
				.Param<bool>("value")
				.Handler((e, v) => e.PrintBool(v));

			scope.Method("Heal")
				.Param<float>("amount")
				.Handler((e, v) => e.Heal(v));

			scope.Method("Damage")
				.Param<float>("amount")
				.Handler((e, v) => e.Damage(v));

			scope.Method<float>("GetRandom")
				.Handler(e => e.GetRandom());
		}
	}
}