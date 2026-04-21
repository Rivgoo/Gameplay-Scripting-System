using GSS.Kernel.Api.Abstractions;
using GSS.Kernel.Api.Builders;

namespace GSS.ConsoleTools.Models
{
	public sealed class AppEnvironmentProfile : ApiProfile<AppEnvironment>
	{
		public override void Configure(IApiBindingScope<AppEnvironment> scope)
		{
			scope.Property<string>("PlayerName")
				.Get(e => e.PlayerName)
				.Set((e, v) => e.PlayerName = v);

			scope.Property<float>("PlayerHealth")
				.Get(e => e.PlayerHealth)
				.Set((e, v) => e.PlayerHealth = v);

			scope.Method("Print")
				.Param<string>("message")
				.Handler((AppEnvironment e, string m) => e.Print(m));

			scope.Method("PrintInt")
				.Param<int>("value")
				.Handler((AppEnvironment e, int v) => e.PrintInt(v));

			scope.Method("PrintFloat")
				.Param<float>("value")
				.Handler((AppEnvironment e, float v) => e.PrintFloat(v));

			scope.Method("PrintBool")
				.Param<bool>("value")
				.Handler((AppEnvironment e, bool v) => e.PrintBool(v));

			scope.Method("Heal")
				.Param<float>("amount")
				.Handler((AppEnvironment e, float v) => e.Heal(v));

			scope.Method("Damage")
				.Param<float>("amount")
				.Handler((AppEnvironment e, float v) => e.Damage(v));

			scope.Method<float>("GetRandom")
				.Handler(e => e.GetRandom());
		}
	}
}