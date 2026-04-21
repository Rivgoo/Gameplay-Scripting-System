namespace GSS.ConsoleTools.Models
{
	public sealed class AppEnvironment
	{
		public string PlayerName { get; set; } = "ConsoleUser";
		public float PlayerHealth { get; set; } = 100f;

		public void Print(string message)
		{
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine($"[Script] {message}");
			Console.ResetColor();
		}

		public void PrintInt(int value) => Print(value.ToString());
		public void PrintFloat(float value) => Print(value.ToString("F2"));
		public void PrintBool(bool value) => Print(value.ToString());

		public void Heal(float amount)
		{
			PlayerHealth += amount;
			Print($"Player healed by {amount}. Health is now {PlayerHealth}.");
		}

		public void Damage(float amount)
		{
			PlayerHealth -= amount;
			Print($"Player took {amount} damage. Health is now {PlayerHealth}.");
			if (PlayerHealth <= 0) Print("Player died.");
		}

		public float GetRandom() => (float)Random.Shared.NextDouble();
	}
}