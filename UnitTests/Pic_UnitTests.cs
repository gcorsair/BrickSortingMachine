using System;
using System.Drawing;
using System.Runtime.InteropServices;
using BrickSorter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BrickSorter
{
    [TestClass]
    public class UnitTest : Img
    {
        //[TestMethod]
        //public void LoadConvertAdjustSave()
        //{
        //    LoadFromFile(@"C:\Users\ViaBid00\Pictures\two_2.png");

        //    ConvertToLowerQuality();
        //    CompensateBackground();
        //    Save(@"C:\Users\ViaBid00\Downloads\two_2_ConvertedAdjusted.png");
        //}
        //[TestMethod]
        //public void FoundCornersAreCorrect()
        //{
        //    LoadFromFile(@"C:\Users\ViaBid00\Downloads\two_2_ConvertedAdjusted.png");

        //    FindLeftCorner();
        //    FindRightCorner();
        //    Assert.AreEqual(new Point(102, 104), _leftCorner);
        //    Assert.AreEqual(new Point(183, 140), _rightCorner);
        //}

        //[TestMethod]
        //public void FoundAngleIsCorrect()
        //{
        //    LoadFromFile(@"C:\Users\ViaBid00\Downloads\two_2_ConvertedAdjusted.png");
        //    _leftCorner = new Point(102, 104);
        //    _rightCorner = new Point(183, 140);
        //    FindAngle();
        //    DrawLine(_leftCorner, Color.Red);
        //    Save(@"C:\Users\ViaBid00\Downloads\two_2_FoundAngle.png");
        //    Assert.AreEqual(41, _angle);
        //}

        //[TestMethod]
        //public void AdjustedRightCornerIsCorrect()
        //{
        //    LoadFromFile(@"C:\Users\ViaBid00\Downloads\two_2_ConvertedAdjusted.png");
        //    _leftCorner = new Point(102, 104);
        //    _rightCorner = new Point(183, 140);
        //    _angle = 41;

        //    AdjustRightCorner();
        //    DrawLine(_rightCorner, Color.Red);
        //    Save(@"C:\Users\ViaBid00\Downloads\two_2_FoundRightCorner.png");
        //    Assert.AreEqual(new Point(183, 140), _rightCorner);
        //}

        //[TestMethod]
        //public void FoundTopCornerIsCorrect()
        //{
        //    LoadFromFile(@"C:\Users\ViaBid00\Downloads\two_2_ConvertedAdjusted.png");
        //    _leftCorner = new Point(102, 104);
        //    _rightCorner = new Point(183, 140);
        //    _angle = 41;

        //    FindTopCorner();
        //    _angle += 90;
        //    DrawLine(_topCorner, Color.Red);
        //    Save(@"C:\Users\ViaBid00\Downloads\two_2_FoundTopCorner.png");
        //    Assert.AreEqual(new Point(101, 103), _topCorner);
        //}

        //[TestMethod]
        //public void FoundBottomCornerIsCorrect()
        //{
        //    LoadFromFile(@"C:\Users\ViaBid00\Downloads\two_2_ConvertedAdjusted.png");
        //    _leftCorner = new Point(102, 104);
        //    _rightCorner = new Point(183, 140);
        //    _angle = 41;

        //    FindBottomBorder();
        //    _angle += 90;
        //    DrawLine(_bottomCorner, Color.Red);
        //    Save(@"C:\Users\ViaBid00\Downloads\two_2_FoundBottomCorner.png");
        //    Assert.AreEqual(new Point(155, 165), _bottomCorner);
        //}

        //[TestMethod]
        //public void FoundLengthIsCorrect()
        //{
        //    LoadFromFile(@"C:\Users\ViaBid00\Downloads\two_2_ConvertedAdjusted.png");
        //    _leftCorner = new Point(102, 104);
        //    _rightCorner = new Point(183, 140);
        //    _topCorner = new Point(101, 103);
        //    _bottomCorner = new Point(155, 165);

        //    CalculateLength();
        //    DrawLine(_topCorner, _bottomCorner, Color.Red);
        //    Save(@"C:\Users\ViaBid00\Downloads\two_2_FoundLength.png");
        //    Assert.AreEqual(82, _length);
        //}

        //[TestMethod]
        //public void FoundWidthIsCorrect()
        //{
        //    LoadFromFile(@"C:\Users\ViaBid00\Downloads\two_2_ConvertedAdjusted.png");
        //    _leftCorner = new Point(102, 104);
        //    _rightCorner = new Point(183, 140);
        //    _topCorner = new Point(101, 103);
        //    _bottomCorner = new Point(155, 165);
        //    _length = 82;

        //    CalculateWidth();
        //    DrawLine(_bottomCorner, _rightCorner, Color.Red);
        //    Save(@"C:\Users\ViaBid00\Downloads\two_2_FoundWidth.png");
        //    Assert.AreEqual(37, _width);
        //}

        //[TestMethod]
        //public void CalculationsAreCorrect()
        //{
        //    SetCalibration(40);
        //    var brickName = GetBrickSize(@"C:\Users\ViaBid00\Pictures\two_2.png");
        //    DrawText(_leftCorner, Color.Red, brickName);
        //    Save(@"C:\Users\ViaBid00\Pictures\two_2_done.png");
        //}
    }
}
