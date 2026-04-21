using FluentAssertions;

namespace Gss.Core.Tests.ApiBinding
{
	[TestFixture]
	public class ApiRegistryTests
	{
		private IApiRegistry _registry;

		[SetUp]
		public void Setup()
		{
			var builder = new ApiRegistryBuilder();
			builder.Register(new CharacterApiProfile());
			_registry = builder.Build();
		}

		[Test]
		public void RetrieveClass_WhenClassExists_ShouldReturnValidMetadata()
		{
			// Act
			var meta = _registry.RetrieveClass("Character");

			// Assert
			meta.Should().NotBeNull();
			meta.Name.Should().Be("Character");
		}

		[Test]
		public void RetrieveClass_WhenClassDoesNotExist_ShouldThrowGssMemberNotFoundException()
		{
			// Act
			Action act = () => _registry.RetrieveClass("NonExistentClass");

			// Assert
			act.Should().Throw<GssMemberNotFoundException>();
		}

		[Test]
		public void RetrieveMethod_WhenMethodExists_ShouldReturnMethodMetadata()
		{
			// Arrange
			var meta = _registry.RetrieveClass("Character");

			// Act
			var method = meta.RetrieveMethod("Respawn");

			// Assert
			method.Should().NotBeNull();
			method.Name.Should().Be("Respawn");
		}

		[Test]
		public void RetrieveMethod_WhenMethodDoesNotExist_ShouldThrowGssMemberNotFoundException()
		{
			// Arrange
			var meta = _registry.RetrieveClass("Character");

			// Act
			Action act = () => meta.RetrieveMethod("NonExistentMethod");

			// Assert
			act.Should().Throw<GssMemberNotFoundException>();
		}

		[Test]
		public void RetrieveProperty_WhenPropertyExists_ShouldReturnPropertyMetadata()
		{
			// Arrange
			var meta = _registry.RetrieveClass("Character");

			// Act
			var prop = meta.RetrieveProperty("Level");

			// Assert
			prop.Should().NotBeNull();
			prop.Name.Should().Be("Level");
		}

		[Test]
		public void RetrieveProperty_WhenPropertyDoesNotExist_ShouldThrowGssMemberNotFoundException()
		{
			// Arrange
			var meta = _registry.RetrieveClass("Character");

			// Act
			Action act = () => meta.RetrieveProperty("NonExistentProperty");

			// Assert
			act.Should().Throw<GssMemberNotFoundException>();
		}
	}
}