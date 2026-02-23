using NUnit.Framework;

namespace HTDA.Framework.Pooling.Tests
{
    public class TemplateRuntimeTests
    {
        [Test]
        public void PackageInfo_Name_IsNotEmpty()
        {
            Assert.IsFalse(string.IsNullOrEmpty(HTDA.Framework.Pooling.PackageInfo.Name));
        }
    }
}