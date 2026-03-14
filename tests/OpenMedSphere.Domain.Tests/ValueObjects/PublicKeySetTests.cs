using OpenMedSphere.Domain.ValueObjects;
using Xunit;

namespace OpenMedSphere.Domain.Tests.ValueObjects
{
    public sealed class PublicKeySetTests
    {
        [Fact]
        public void Create_WithValidKeys_ReturnsPublicKeySet()
        {
            var result = PublicKeySet.Create("mlKem", "mlDsa", "x25519", "ecdsa", 1);

            Assert.NotNull(result);
            Assert.Equal("mlKem", result.MlKemPublicKey);
            Assert.Equal("mlDsa", result.MlDsaPublicKey);
            Assert.Equal("x25519", result.X25519PublicKey);
            Assert.Equal("ecdsa", result.EcdsaPublicKey);
            Assert.Equal(1, result.KeyVersion);
        }

        [Fact]
        public void Create_WithNullMlKemKey_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                PublicKeySet.Create(null!, "mlDsa", "x25519", "ecdsa", 1));
        }

        [Fact]
        public void Create_WithWhitespaceMlKemKey_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                PublicKeySet.Create("  ", "mlDsa", "x25519", "ecdsa", 1));
        }

        [Fact]
        public void Create_WithNullMlDsaKey_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                PublicKeySet.Create("mlKem", null!, "x25519", "ecdsa", 1));
        }

        [Fact]
        public void Create_WithNullX25519Key_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                PublicKeySet.Create("mlKem", "mlDsa", null!, "ecdsa", 1));
        }

        [Fact]
        public void Create_WithNullEcdsaKey_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                PublicKeySet.Create("mlKem", "mlDsa", "x25519", null!, 1));
        }

        [Fact]
        public void Create_WithZeroKeyVersion_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                PublicKeySet.Create("mlKem", "mlDsa", "x25519", "ecdsa", 0));
        }

        [Fact]
        public void Create_WithNegativeKeyVersion_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                PublicKeySet.Create("mlKem", "mlDsa", "x25519", "ecdsa", -1));
        }

        [Fact]
        public void Equality_SameValues_AreEqual()
        {
            var keys1 = PublicKeySet.Create("mlKem", "mlDsa", "x25519", "ecdsa", 1);
            var keys2 = PublicKeySet.Create("mlKem", "mlDsa", "x25519", "ecdsa", 1);

            Assert.Equal(keys1, keys2);
        }

        [Fact]
        public void Equality_DifferentKeyVersion_AreNotEqual()
        {
            var keys1 = PublicKeySet.Create("mlKem", "mlDsa", "x25519", "ecdsa", 1);
            var keys2 = PublicKeySet.Create("mlKem", "mlDsa", "x25519", "ecdsa", 2);

            Assert.NotEqual(keys1, keys2);
        }
    }
}
