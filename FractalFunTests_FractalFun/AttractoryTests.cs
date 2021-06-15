using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace FractalFun.Tests_FractalFun
{
    [TestClass()]
    public class AttractoryTests
    {
        [TestMethod()]
        public void AttractoryTest()
        {
            Assert.AreSame(1,1);
        }

        [TestMethod()]
        public void LoadPredefinesTest()
        {
            Attractor Attractors = new Attractor();

            using (StreamReader r = new StreamReader("PredefinedAttractors.json"))
            {
                string json = r.ReadToEnd();
                //Attractors = JsonConvert.DeserializeObject<List<Attractor>>(json);
            }
            Assert.Fail();
        }

        [TestMethod()]
        public void SetOrResetUITest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DoResetTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DoResizeTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CanRenderTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CanLoopTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetBeginTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetEndTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetStepTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetParamIndexTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DoLoopRenderTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DoRenderTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void MakePaperTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetColorTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DoSaveFileTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void FormatDTTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SaveLogTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CheckDTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CheckITest()
        {
            Assert.Fail();
        }
    }
}