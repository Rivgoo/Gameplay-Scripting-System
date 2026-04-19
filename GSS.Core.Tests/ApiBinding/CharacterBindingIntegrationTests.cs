using FluentAssertions;
using GSS.Core.ApiBinding.Exceptions;
using GSS.Core.ApiBinding.Models;
using GSS.Core.ApiBinding.Registry;
using GSS.Core.ApiBinding.Abstractions;

namespace Gss.Core.Tests.ApiBinding
{
	[TestFixture]
	public class CharacterBindingIntegrationTests
	{
		private IClassMetadata _meta;
		private Character _character;

		[SetUp]
		public void Setup()
		{
			var builder = new ApiRegistryBuilder();
			builder.Register(new CharacterApiProfile());
			_meta = builder.Build().RetrieveClass("Character");
			_character = new Character();
		}

		[Test]
		public void PropertyAccess_SetAndGet_Int_ShouldUpdateValueSuccessfully()
		{
			// Arrange
			var prop = _meta.RetrieveProperty("Level");

			// Act
			prop.SetValue(_character, GssValue.FromInt(50));
			var result = prop.GetValue(_character);

			// Assert
			_character.Level.Should().Be(50);
			result.Type.Should().Be(GssType.Int);
			result.AsInt.Should().Be(50);
		}

		[Test]
		public void PropertyAccess_SetAndGet_String_ShouldUpdateValueSuccessfully()
		{
			// Arrange
			var prop = _meta.RetrieveProperty("Name");

			// Act
			prop.SetValue(_character, GssValue.FromObject("Hero"));
			var result = prop.GetValue(_character);

			// Assert
			_character.Name.Should().Be("Hero");
			result.Type.Should().Be(GssType.Object);
			result.AsObject.Should().Be("Hero");
		}

		[Test]
		public void PropertyAccess_SetAndGet_Float_ShouldUpdateValueSuccessfully()
		{
			// Arrange
			var prop = _meta.RetrieveProperty("Health");

			// Act
			prop.SetValue(_character, GssValue.FromFloat(85.5f));
			var result = prop.GetValue(_character);

			// Assert
			_character.Health.Should().Be(85.5f);
			result.Type.Should().Be(GssType.Float);
			result.AsFloat.Should().Be(85.5f);
		}

		[Test]
		public void PropertyAccess_SetAndGet_Long_ShouldUpdateValueSuccessfully()
		{
			// Arrange
			var prop = _meta.RetrieveProperty("TotalExp");

			// Act
			prop.SetValue(_character, GssValue.FromLong(15000L));
			var result = prop.GetValue(_character);

			// Assert
			_character.TotalExp.Should().Be(15000L);
			result.Type.Should().Be(GssType.Long);
			result.AsLong.Should().Be(15000L);
		}

		[Test]
		public void PropertyAccess_SetAndGet_Double_ShouldUpdateValueSuccessfully()
		{
			// Arrange
			var prop = _meta.RetrieveProperty("CritRate");

			// Act
			prop.SetValue(_character, GssValue.FromDouble(0.75));
			var result = prop.GetValue(_character);

			// Assert
			_character.CritRate.Should().Be(0.75);
			result.Type.Should().Be(GssType.Double);
			result.AsDouble.Should().Be(0.75);
		}

		[Test]
		public void PropertyAccess_SetAndGet_Bool_ShouldUpdateValueSuccessfully()
		{
			// Arrange
			var prop = _meta.RetrieveProperty("IsActive");

			// Act
			prop.SetValue(_character, GssValue.FromBool(false));
			var result = prop.GetValue(_character);

			// Assert
			_character.IsActive.Should().BeFalse();
			result.Type.Should().Be(GssType.Bool);
			result.AsBool.Should().BeFalse();
		}

		[Test]
		public void PropertyAccess_SetAndGet_Enum_ShouldUpdateValueSuccessfully()
		{
			// Arrange
			var prop = _meta.RetrieveProperty("Element");

			// Act
			prop.SetValue(_character, GssValue.FromObject(ElementType.Water));
			var result = prop.GetValue(_character);

			// Assert
			_character.Element.Should().Be(ElementType.Water);
			result.AsObject.Should().Be(ElementType.Water);
		}

		[Test]
		public void PropertyAccess_GetReadOnly_ShouldReturnCurrentValue()
		{
			// Arrange
			var prop = _meta.RetrieveProperty("Id");
			var expectedId = _character.Id;

			// Act
			var result = prop.GetValue(_character);

			// Assert
			result.AsObject.Should().Be(expectedId);
		}

		[Test]
		public void PropertySet_WhenPropertyIsReadOnly_ShouldThrowGssMemberNotFoundException()
		{
			// Arrange
			var prop = _meta.RetrieveProperty("Id");

			// Act
			Action act = () => prop.SetValue(_character, GssValue.FromObject("NewId"));

			// Assert
			act.Should().Throw<GssMemberNotFoundException>();
		}

		[Test]
		public void PropertySet_WhenTypeMismatches_ShouldThrowGssTypeMismatchException()
		{
			// Arrange
			var prop = _meta.RetrieveProperty("Level");

			// Act: Passing float to an int property
			Action act = () => prop.SetValue(_character, GssValue.FromFloat(50.5f));

			// Assert
			act.Should().Throw<GssTypeMismatchException>();
		}

		[Test]
		public void PropertySet_WhenValueOutOfRange_ShouldThrowGssValidationException()
		{
			// Arrange
			var prop = _meta.RetrieveProperty("Level");

			// Act
			Action act = () => prop.SetValue(_character, GssValue.FromInt(999));

			// Assert
			act.Should().Throw<GssValidationException>();
		}

		[Test]
		public void PropertySet_WhenStringIsEmpty_ShouldThrowGssValidationException()
		{
			// Arrange
			var prop = _meta.RetrieveProperty("Name");

			// Act
			Action act = () => prop.SetValue(_character, GssValue.FromObject("   "));

			// Assert
			act.Should().Throw<GssValidationException>();
		}

		[Test]
		public void PropertySet_WhenFloatIsNegative_ShouldThrowGssValidationException()
		{
			// Arrange
			var prop = _meta.RetrieveProperty("Health");

			// Act
			Action act = () => prop.SetValue(_character, GssValue.FromFloat(-10f));

			// Assert
			act.Should().Throw<GssValidationException>();
		}

		[Test]
		public void MethodInvoke_ArityZero_ShouldExecuteSuccessfully()
		{
			// Arrange
			var method = _meta.RetrieveMethod("Respawn");
			_character.Health = 10f;

			// Act
			method.Invoke(_character, ReadOnlySpan<GssValue>.Empty);

			// Assert
			_character.Health.Should().Be(100f);
		}

		[Test]
		public void MethodInvoke_ArityOne_ShouldExecuteSuccessfully()
		{
			// Arrange
			var method = _meta.RetrieveMethod("Heal");
			_character.Health = 50f;
			var args = new[] { GssValue.FromFloat(25f) };

			// Act
			method.Invoke(_character, args);

			// Assert
			_character.Health.Should().Be(75f);
		}

		[Test]
		public void MethodInvoke_ArityThree_ShouldExecuteSuccessfully()
		{
			// Arrange
			var method = _meta.RetrieveMethod("Move");
			var args = new[] {
				GssValue.FromFloat(10f),
				GssValue.FromFloat(20f),
				GssValue.FromFloat(30f)
			};

			// Act
			method.Invoke(_character, args);

			// Assert
			_character.Position.x.Should().Be(10f);
			_character.Position.y.Should().Be(20f);
			_character.Position.z.Should().Be(30f);
		}

		[Test]
		public void MethodInvoke_ArityFive_ShouldReturnExpectedResult()
		{
			// Arrange
			var method = _meta.RetrieveMethod("Ultimate");
			var args = new[] {
				GssValue.FromInt(50),
				GssValue.FromFloat(2.0f),
				GssValue.FromBool(true), // triggers critical multiplier (* 2)
				GssValue.FromObject("boom_sfx"),
				GssValue.FromObject(ElementType.Fire)
			};

			// Act
			var result = method.Invoke(_character, args);

			// Assert: 50 * 2.0 * 2 = 200
			result.AsFloat.Should().Be(200f);
		}

		[Test]
		public void MethodInvoke_WhenPassingTooFewArguments_ShouldThrowGssArgumentCountMismatchException()
		{
			// Arrange
			var method = _meta.RetrieveMethod("Ultimate");
			var args = new[] { GssValue.FromInt(50) }; // Expects 5

			// Act
			Action act = () => method.Invoke(_character, args);

			// Assert
			act.Should().Throw<GssArgumentCountMismatchException>();
		}

		[Test]
		public void MethodInvoke_WhenPassingTooManyArguments_ShouldThrowGssArgumentCountMismatchException()
		{
			// Arrange
			var method = _meta.RetrieveMethod("Respawn");
			var args = new[] { GssValue.FromInt(1) }; // Expects 0

			// Act
			Action act = () => method.Invoke(_character, args);

			// Assert
			act.Should().Throw<GssArgumentCountMismatchException>();
		}

		[Test]
		public void MethodInvoke_WhenArgumentTypeMismatches_ShouldThrowGssTypeMismatchException()
		{
			// Arrange
			var method = _meta.RetrieveMethod("Heal");
			var args = new[] { GssValue.FromInt(20) }; // Expects float

			// Act
			Action act = () => method.Invoke(_character, args);

			// Assert
			act.Should().Throw<GssTypeMismatchException>();
		}

		[Test]
		public void MethodInvoke_WhenArgumentFailsValidation_ShouldThrowGssValidationException()
		{
			// Arrange
			var method = _meta.RetrieveMethod("Heal");
			// Amount must be positive. Passing 0 is invalid.
			var args = new[] { GssValue.FromFloat(0f) };

			// Act
			Action act = () => method.Invoke(_character, args);

			// Assert
			act.Should().Throw<GssValidationException>();
		}

		[Test]
		public void MethodInvoke_WhenStringArgumentIsEmpty_ShouldThrowGssValidationException()
		{
			// Arrange
			var method = _meta.RetrieveMethod("SetAttr");
			var args = new[] {
				GssValue.FromObject(""), // Key cannot be empty
				GssValue.FromFloat(10f)
			};

			// Act
			Action act = () => method.Invoke(_character, args);

			// Assert
			act.Should().Throw<GssValidationException>();
		}
	}
}