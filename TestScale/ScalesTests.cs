using Microsoft.VisualStudio.TestTools.UnitTesting;
using MusicScale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestScale
{
    [TestClass]
    public class ScalesTests
    {
        [TestMethod]
        public void AllScalesDifferent()
        {
            for (int i = 0; i < Scales.All.Length - 1; i++)
                for (int j = i + 1; j < Scales.All.Length; j++)
                    if (Scales.All[i] == Scales.All[j])
                        Assert.Fail("Scales equal:" + Environment.NewLine + Scales.All[i] + Environment.NewLine + Scales.All[j]);
        }

        [TestMethod]
        public void NoTripleOneDoubleZeroScales()
        {
            for (int i = 0; i < Scales.All.Length; i++)
            {
                int ones = 0;
                int zeroes = 0;
                int len = 0;
                ulong mask = Scales.All[i].Mask;
                for (int j = 0; j < Common.MaskLength; j++)
                {
                    if ((mask % 2) == 0)
                    {
                        if (++zeroes > 1)
                            Assert.Fail("Scale contains two consecutive zeroes: " + Scales.All[i]);
                        ones = 0;
                    }
                    else
                    {
                        if (++ones > 2)
                            Assert.Fail("Scale contains three consecutive ones: " + Scales.All[i]);
                        zeroes = 0;
                    }
                    len++;
                    mask /= 2;
                }
            }
        }
    }
}
