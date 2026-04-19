using GSS.Core.ApiBinding.Abstractions;
using GSS.Core.ApiBinding.Builders;
using GSS.Core.ApiBinding.Validation;

namespace Gss.Core.Tests.ApiBinding
{
	public class CharacterApiProfile : ApiProfile<Character>
	{
		public override void Build(IApiBindingScope<Character> scope)
		{
			// Properties
			scope.Property<string>("Id")
				.Get(c => c.Id);

			scope.Property<string>("Name")
				.Get(c => c.Name)
				.Set((c, v) => c.Name = v)
				.MustNotBeNull()
				.MustNotBeEmpty()
				.MaxLength(50);

			scope.Property<int>("Level")
				.Get(c => c.Level)
				.Set((c, v) => c.Level = v)
				.MustBeInRange(1, 100);

			scope.Property<float>("Health")
				.Get(c => c.Health)
				.Set((c, v) => c.Health = v)
				.MustBeNonNegative()
				.MustBeValidNumber();

			scope.Property<float>("Mana")
				.Get(c => c.Mana)
				.Set((c, v) => c.Mana = v)
				.MustBeNonNegative()
				.MustBeValidNumber();

			scope.Property<long>("TotalExp")
				.Get(c => c.TotalExp)
				.Set((c, v) => c.TotalExp = v)
				.MustBeNonNegative();

			scope.Property<double>("CritRate")
				.Get(c => c.CritRate)
				.Set((c, v) => c.CritRate = v)
				.MustBeInRange(0.0, 1.0);

			scope.Property<bool>("IsActive")
				.Get(c => c.IsActive)
				.Set((c, v) => c.IsActive = v);

			scope.Property<ElementType>("Element")
				.Get(c => c.Element)
				.Set((c, v) => c.Element = v)
				.MustBeValidEnum();

			// Methods
			scope.Method("Respawn")
				.Handler(c => c.Respawn());

			scope.Method<string>("GetIdentity")
				.Handler(c => c.GetIdentity());

			scope.Method("Heal")
				.Param<float>("amount", v => v.MustBePositive().MustBeValidNumber())
				.Handler((c, amt) => c.ApplyHeal(amt));

			scope.Method<bool>("SetAttr")
				.Param<string>("key", v => v.MustNotBeNull().MustNotBeEmpty())
				.Param<float>("val", v => v.MustBeValidNumber())
				.Handler((c, k, v) => c.SetAttribute(k, v));

			scope.Method("Move")
				.Param<float>("x", v => v.MustBeValidNumber())
				.Param<float>("y", v => v.MustBeValidNumber())
				.Param<float>("z", v => v.MustBeValidNumber())
				.Handler((c, x, y, z) => c.MoveTo(x, y, z));

			scope.Method<string>("Buff")
				.Param<string>("name", v => v.MustNotBeEmpty())
				.Param<float>("dur", v => v.MustBePositive())
				.Param<int>("pwr", v => v.MustBePositive())
				.Param<bool>("stack")
				.Handler((c, n, d, p, s) => c.FormatBuff(n, d, p, s));

			scope.Method<float>("Ultimate")
				.Param<int>("energy", v => v.MustBeInRange(0, 100))
				.Param<float>("mult", v => v.MustBePositive())
				.Param<bool>("crit")
				.Param<string>("sfx", v => v.MustNotBeNull())
				.Param<ElementType>("type", v => v.MustBeValidEnum())
				.Handler((c, e, m, cr, s, t) => c.ExecuteUltimate(e, m, cr, s, t));
		}
	}
}