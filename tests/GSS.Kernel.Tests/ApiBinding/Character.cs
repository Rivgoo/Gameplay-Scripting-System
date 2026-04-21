namespace Gss.Core.Tests.ApiBinding
{
	public class Character
	{
		public string Id { get; } = Guid.NewGuid().ToString();
		public string Name = "Unknown";
		public int Level = 1;
		public float Health = 100f;
		public float Mana = 50f;
		public long TotalExp = 0;
		public double CritRate = 0.05;
		public bool IsActive = true;
		public ElementType Element = ElementType.Fire;

		public struct Vector3Sim { public float x, y, z; }
		public Vector3Sim Position;

		public void Respawn() { Health = 100f; IsActive = true; }
		public string GetIdentity() => $"[{Id}] {Name}";

		public void ApplyHeal(float amount) => Health = Math.Min(100f, Health + amount);

		public bool SetAttribute(string key, float value) => value > 0;

		public void MoveTo(float x, float y, float z)
		{
			Position = new Vector3Sim { x = x, y = y, z = z };
		}

		public string FormatBuff(string buffName, float duration, int power, bool isStackable)
			=> $"{buffName}: Pwr({power}) Dur({duration}) Stack({isStackable})";

		public float ExecuteUltimate(int energy, float mult, bool critical, string sfx, ElementType type)
		{
			if (!critical) return energy * mult;
			return energy * mult * 2f;
		}
	}
}