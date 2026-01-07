using Xunit;

namespace Avalonia.Controls.PanAndZoom.UnitTests;

public class MatrixHelperTests
{
    [Fact]
    public void Translate_Returns_Matrix()
    {
        var target = MatrixHelper.Translate(20, 30);
        Assert.Equal(1.0, target.M11);
        Assert.Equal(0.0, target.M12);
        Assert.Equal(0.0, target.M21);
        Assert.Equal(1.0, target.M22);
        Assert.Equal(20.0, target.M31);
        Assert.Equal(30.0, target.M32);
    }

    [Fact]
    public void Scale_Returns_Matrix()
    {
        var target = MatrixHelper.Scale(2, 3);
        Assert.Equal(2.0, target.M11);
        Assert.Equal(0.0, target.M12);
        Assert.Equal(0.0, target.M21);
        Assert.Equal(3.0, target.M22);
        Assert.Equal(0.0, target.M31);
        Assert.Equal(0.0, target.M32);
    }

    [Fact]
    public void Transform_Point_Identity()
    {
        var inputPoint = new Point(0, 0);
        var testMatrix = new Matrix();
        var outputPoint = MatrixHelper.TransformPoint(testMatrix, inputPoint);

        Assert.Equal(0, inputPoint.X);
        Assert.Equal(0, inputPoint.Y);

        Assert.Equal(0, outputPoint.X);
        Assert.Equal(0, outputPoint.Y);
    }

    [Fact]
    public void Transform_Point_Positive()
    {
        var inputPoint = new Point(2, 1);
        var testMatrix = new Matrix(1.0, 0.0, 0.0, 2.0, 0.0, 0.0);
        var outputPoint = MatrixHelper.TransformPoint(testMatrix, inputPoint);

        Assert.Equal(2, inputPoint.X);
        Assert.Equal(1, inputPoint.Y);

        Assert.Equal(2, outputPoint.X);
        Assert.Equal(2, outputPoint.Y);
    }

    [Fact]
    public void ScaleAt_Same_Returns_Identity()
    {
        double scaleX = 1.0;
        double scaleY = 1.0;
        double centerX = 0.0;
        double centerY = 0.0;
        var outputMatrix = MatrixHelper.ScaleAt(scaleX, scaleY, centerX, centerY);
        Assert.True(outputMatrix.IsIdentity);
    }

    [Fact]
    public void ScaleAt_SameScale_PositiveCenter_Returns_Identity()
    {
        double scaleX = 1.0;
        double scaleY = 1.0;
        double centerX = 1.0;
        double centerY = 2.0;
        var outputMatrix = MatrixHelper.ScaleAt(scaleX, scaleY, centerX, centerY);
        Assert.True(outputMatrix.IsIdentity);
    }

    [Fact]
    public void ScaleAt_DoubleScale_PositiveCenter()
    {
        double scaleX = 2.0;
        double scaleY = 2.0;
        double centerX = 1.0;
        double centerY = 2.0;
        var outputMatrix = MatrixHelper.ScaleAt(scaleX, scaleY, centerX, centerY);

        Assert.Equal(2.0, outputMatrix.M11);
        Assert.Equal(0.0, outputMatrix.M12);
        Assert.Equal(0.0, outputMatrix.M13);
        Assert.Equal(0.0, outputMatrix.M21);
        Assert.Equal(2.0, outputMatrix.M22);
        Assert.Equal(0.0, outputMatrix.M23);
        Assert.Equal(-1.0, outputMatrix.M31);
        Assert.Equal(-2.0, outputMatrix.M32);
        Assert.Equal(1.0, outputMatrix.M33);
    }

    [Fact]
    public void ScaleAt_HalfScale_NegCenter()
    {
        double scaleX = 0.5;
        double scaleY = 0.5;
        double centerX = -1.0;
        double centerY = -2.0;
        var outputMatrix = MatrixHelper.ScaleAt(scaleX, scaleY, centerX, centerY);

        Assert.Equal(0.5, outputMatrix.M11);
        Assert.Equal(0.0, outputMatrix.M12);
        Assert.Equal(0.0, outputMatrix.M13);
        Assert.Equal(0.0, outputMatrix.M21);
        Assert.Equal(0.5, outputMatrix.M22);
        Assert.Equal(0.0, outputMatrix.M23);
        Assert.Equal(-0.5, outputMatrix.M31);
        Assert.Equal(-1.0, outputMatrix.M32);
        Assert.Equal(1.0, outputMatrix.M33);
    }

    [Fact]
    public void ScaleAndTranslate_Same_Returns_Identity()
    {
        double scaleX = 1.0;
        double scaleY = 1.0;
        double x = 0.0;
        double y = 0.0;
        var outputMatrix = MatrixHelper.ScaleAndTranslate(scaleX, scaleY, x, y);
        Assert.True(outputMatrix.IsIdentity);
    }

    [Fact]
    public void ScaleAndTranslate_DoubleScale_PositiveShift()
    {
        double scaleX = 2.0;
        double scaleY = 2.0;
        double x = 1.0;
        double y = 2.0;
        var outputMatrix = MatrixHelper.ScaleAndTranslate(scaleX, scaleY, x, y);

        Assert.Equal(2.0, outputMatrix.M11);
        Assert.Equal(0.0, outputMatrix.M12);
        Assert.Equal(0.0, outputMatrix.M13);
        Assert.Equal(0.0, outputMatrix.M21);
        Assert.Equal(2.0, outputMatrix.M22);
        Assert.Equal(0.0, outputMatrix.M23);
        Assert.Equal(1.0, outputMatrix.M31);
        Assert.Equal(2.0, outputMatrix.M32);
        Assert.Equal(1.0, outputMatrix.M33);
    }

    [Fact]
    public void ScaleAndTranslate_HalfScale_NegativeShift()
    {
        double scaleX = 0.5;
        double scaleY = 0.5;
        double x = -1.0;
        double y = -2.0;
        var outputMatrix = MatrixHelper.ScaleAndTranslate(scaleX, scaleY, x, y);

        Assert.Equal(0.5, outputMatrix.M11);
        Assert.Equal(0.0, outputMatrix.M12);
        Assert.Equal(0.0, outputMatrix.M13);
        Assert.Equal(0.0, outputMatrix.M21);
        Assert.Equal(0.5, outputMatrix.M22);
        Assert.Equal(0.0, outputMatrix.M23);
        Assert.Equal(-1.0, outputMatrix.M31);
        Assert.Equal(-2.0, outputMatrix.M32);
        Assert.Equal(1.0, outputMatrix.M33);
    }

    [Fact]
    public void ScaleAtPrepend_HalfScale_With_DoubleScale_Returns_Identity()
    {
        var halfScaleMatrix = MatrixHelper.Scale(0.5, 0.5);

        double scaleX = 2.0;
        double scaleY = 2.0;
        var outputMatrix = MatrixHelper.ScaleAtPrepend(halfScaleMatrix, scaleX, scaleY, 0, 0);

        Assert.True(outputMatrix.IsIdentity);
    }

    [Fact]
    public void Rotation_Zero_Returns_Identity()
    {
        // Arrange
        double radians = 0.0;
        
        // Act
        var outputMatrix = MatrixHelper.Rotation(radians);
        
        // Assert
        Assert.True(outputMatrix.IsIdentity);
    }

    [Fact]
    public void Rotation_90Degrees_Returns_CorrectMatrix()
    {
        // Arrange
        double radians = System.Math.PI / 2.0; // 90 degrees
        
        // Act
        var outputMatrix = MatrixHelper.Rotation(radians);
        
        // Assert - For 90 degree rotation: cos=0, sin=1
        Assert.Equal(0.0, outputMatrix.M11, 10); // cos(90) = 0
        Assert.Equal(1.0, outputMatrix.M12, 10); // sin(90) = 1
        Assert.Equal(-1.0, outputMatrix.M21, 10); // -sin(90) = -1
        Assert.Equal(0.0, outputMatrix.M22, 10); // cos(90) = 0
        Assert.Equal(0.0, outputMatrix.M31);
        Assert.Equal(0.0, outputMatrix.M32);
    }

    [Fact]
    public void Rotation_180Degrees_Returns_CorrectMatrix()
    {
        // Arrange
        double radians = System.Math.PI; // 180 degrees
        
        // Act
        var outputMatrix = MatrixHelper.Rotation(radians);
        
        // Assert - For 180 degree rotation: cos=-1, sin=0
        Assert.Equal(-1.0, outputMatrix.M11, 10); // cos(180) = -1
        Assert.Equal(0.0, outputMatrix.M12, 10); // sin(180) = 0
        Assert.Equal(0.0, outputMatrix.M21, 10); // -sin(180) = 0
        Assert.Equal(-1.0, outputMatrix.M22, 10); // cos(180) = -1
        Assert.Equal(0.0, outputMatrix.M31);
        Assert.Equal(0.0, outputMatrix.M32);
    }

    [Fact]
    public void Rotation_45Degrees_Returns_CorrectMatrix()
    {
        // Arrange
        double radians = System.Math.PI / 4.0; // 45 degrees
        double expected = System.Math.Sqrt(2) / 2.0; // ~0.707
        
        // Act
        var outputMatrix = MatrixHelper.Rotation(radians);
        
        // Assert - For 45 degree rotation: cos=sin=sqrt(2)/2
        Assert.Equal(expected, outputMatrix.M11, 10);
        Assert.Equal(expected, outputMatrix.M12, 10);
        Assert.Equal(-expected, outputMatrix.M21, 10);
        Assert.Equal(expected, outputMatrix.M22, 10);
    }

    [Fact]
    public void Rotation_WithCenter_90Degrees_TransformsPointCorrectly()
    {
        // Arrange - Rotate 90 degrees around center point (1, 1)
        double radians = System.Math.PI / 2.0;
        double centerX = 1.0;
        double centerY = 1.0;
        
        // Act
        var outputMatrix = MatrixHelper.Rotation(radians, centerX, centerY);
        
        // Transform point (2, 1) which is 1 unit to the right of center
        // After 90 degree rotation, it should be at (1, 2) - 1 unit below center
        var testPoint = new Point(2, 1);
        var transformedPoint = MatrixHelper.TransformPoint(outputMatrix, testPoint);
        
        // Assert
        Assert.Equal(1.0, transformedPoint.X, 10);
        Assert.Equal(2.0, transformedPoint.Y, 10);
    }

    [Fact]
    public void Rotation_WithCenter_PointAtCenterUnchanged()
    {
        // Arrange - Rotate 90 degrees around center point (5, 5)
        double radians = System.Math.PI / 2.0;
        double centerX = 5.0;
        double centerY = 5.0;
        
        // Act
        var outputMatrix = MatrixHelper.Rotation(radians, centerX, centerY);
        
        // The center point should remain at center after any rotation
        var testPoint = new Point(5, 5);
        var transformedPoint = MatrixHelper.TransformPoint(outputMatrix, testPoint);
        
        // Assert
        Assert.Equal(5.0, transformedPoint.X, 10);
        Assert.Equal(5.0, transformedPoint.Y, 10);
    }

    [Fact]
    public void Rotation_WithVectorCenter_90Degrees_TransformsPointCorrectly()
    {
        // Arrange - Rotate 90 degrees around center point (1, 1)
        double radians = System.Math.PI / 2.0;
        var center = new Vector(1.0, 1.0);
        
        // Act
        var outputMatrix = MatrixHelper.Rotation(radians, center);
        
        // Transform point (2, 1) which is 1 unit to the right of center
        // After 90 degree rotation, it should be at (1, 2) - 1 unit below center
        var testPoint = new Point(2, 1);
        var transformedPoint = MatrixHelper.TransformPoint(outputMatrix, testPoint);
        
        // Assert
        Assert.Equal(1.0, transformedPoint.X, 10);
        Assert.Equal(2.0, transformedPoint.Y, 10);
    }

    [Fact]
    public void Rotation_Negative90Degrees_Returns_CorrectMatrix()
    {
        // Arrange
        double radians = -System.Math.PI / 2.0; // -90 degrees
        
        // Act
        var outputMatrix = MatrixHelper.Rotation(radians);
        
        // Assert - For -90 degree rotation: cos=0, sin=-1
        Assert.Equal(0.0, outputMatrix.M11, 10); // cos(-90) = 0
        Assert.Equal(-1.0, outputMatrix.M12, 10); // sin(-90) = -1
        Assert.Equal(1.0, outputMatrix.M21, 10); // -sin(-90) = 1
        Assert.Equal(0.0, outputMatrix.M22, 10); // cos(-90) = 0
    }

    #region Skew Tests

    [Fact]
    public void Skew_Zero_Returns_Identity()
    {
        // Arrange & Act
        var outputMatrix = MatrixHelper.Skew(0f, 0f);
        
        // Assert - Skew with zero angles should be identity
        Assert.Equal(1.0, outputMatrix.M11);
        Assert.Equal(0.0, outputMatrix.M12, 10);
        Assert.Equal(0.0, outputMatrix.M21, 10);
        Assert.Equal(1.0, outputMatrix.M22);
        Assert.Equal(0.0, outputMatrix.M31);
        Assert.Equal(0.0, outputMatrix.M32);
    }

    [Fact]
    public void Skew_PositiveAngleX_ReturnsCorrectMatrix()
    {
        // Arrange - Skew by 45 degrees (PI/4 radians) along X axis
        float angleX = (float)(System.Math.PI / 4.0);
        float angleY = 0f;
        
        // Act
        var outputMatrix = MatrixHelper.Skew(angleX, angleY);
        
        // Assert - For 45 degree skew: tan(45) = 1
        Assert.Equal(1.0, outputMatrix.M11);
        Assert.Equal(1.0, outputMatrix.M12, 5); // tan(45) = 1
        Assert.Equal(0.0, outputMatrix.M21, 10);
        Assert.Equal(1.0, outputMatrix.M22);
    }

    [Fact]
    public void Skew_PositiveAngleY_ReturnsCorrectMatrix()
    {
        // Arrange - Skew by 45 degrees along Y axis
        float angleX = 0f;
        float angleY = (float)(System.Math.PI / 4.0);
        
        // Act
        var outputMatrix = MatrixHelper.Skew(angleX, angleY);
        
        // Assert - For 45 degree skew: tan(45) = 1
        Assert.Equal(1.0, outputMatrix.M11);
        Assert.Equal(0.0, outputMatrix.M12, 10);
        Assert.Equal(1.0, outputMatrix.M21, 5); // tan(45) = 1
        Assert.Equal(1.0, outputMatrix.M22);
    }

    [Fact]
    public void Skew_BothAngles_ReturnsCorrectMatrix()
    {
        // Arrange - Skew by 30 degrees along both axes
        float angle = (float)(System.Math.PI / 6.0); // 30 degrees
        
        // Act
        var outputMatrix = MatrixHelper.Skew(angle, angle);
        
        // Assert - tan(30) ≈ 0.577
        double expectedTan = System.Math.Tan(System.Math.PI / 6.0);
        Assert.Equal(1.0, outputMatrix.M11);
        Assert.Equal(expectedTan, outputMatrix.M12, 5);
        Assert.Equal(expectedTan, outputMatrix.M21, 5);
        Assert.Equal(1.0, outputMatrix.M22);
    }

    [Fact]
    public void Skew_NegativeAngles_ReturnsCorrectMatrix()
    {
        // Arrange - Skew by -45 degrees along X axis
        float angleX = (float)(-System.Math.PI / 4.0);
        float angleY = 0f;
        
        // Act
        var outputMatrix = MatrixHelper.Skew(angleX, angleY);
        
        // Assert - tan(-45) = -1
        Assert.Equal(1.0, outputMatrix.M11);
        Assert.Equal(-1.0, outputMatrix.M12, 5);
        Assert.Equal(0.0, outputMatrix.M21, 10);
        Assert.Equal(1.0, outputMatrix.M22);
    }

    #endregion
}
