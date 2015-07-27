using System;
using System.Diagnostics;
using System.Drawing;
using Windows = System.Windows;

namespace SpriteBook
{
    public static class SpriteGenerator
    {
        public static void GenerateSpriteSheet(string[] paths, string outPath, float scale)
        {
            var columns = 0;
            int rows, targetWidth, targetHeight;
            Bitmap targetImage = null;
            Graphics graphics = null;

            try
            {
                var spriteSize = Size.Empty;
                int row = 0, column = 0;
                foreach (var path in paths)
                {
                    using (var image = Bitmap.FromFile(path))
                    {
                        // Check if first image
                        if (spriteSize == Size.Empty)
                        {
                            // Calculate the size of the target image
                            spriteSize = new Size(image.Width, image.Height);
                            columns = SpriteGenerator.CalculateColumns(Convert.ToInt32(Math.Sqrt(paths.Length)), spriteSize.Width);
                            rows = paths.Length / columns;
                            rows += (paths.Length % columns) > 0 ? 1 : 0;
                            targetWidth = columns * spriteSize.Width;
                            targetHeight = rows * spriteSize.Height;

                            // Create the target image
                            targetImage = new Bitmap(targetWidth, targetHeight);
                            targetImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
                            graphics = Graphics.FromImage(targetImage);
                        }
                        else
                        {
                            // Verify each sprite is of the same dimensions
                            if (spriteSize != image.Size)
                                throw new SpriteSizeException();
                        }

                        // Draw the current image to the target
                        graphics.DrawImage(image, column * spriteSize.Width, row * spriteSize.Height);

                        // Move to next row/column
                        if (column >= columns - 1)
                        { column = 0; row++; }
                        else
                            column++;
                    }
                }
                if (targetImage != null)
                {
                    targetImage.Save(outPath);
                    Windows.MessageBox.Show("Sprite sheet successfully written.", "Saved");
                }
                else
                    Windows.MessageBox.Show("No images selected.", "Error");
            }
            finally
            {
                if (targetImage != null)
                    targetImage.Dispose();
            }
        }

        /// <summary>
        /// Calculates the number of columns in such a way that the the remaining space after adding
        /// power-of-two padding is less than the width of a single frame at the current size and half size
        /// </summary>
        private static int CalculateColumns(int defaultColumnCount, int frameWidth)
        {
            // Reduce just to attemp to find a less wide match
            if (defaultColumnCount > 2)
                defaultColumnCount -= 1;
            while ((SpriteGenerator.GetNextPowerOfTwo(defaultColumnCount * frameWidth) - (defaultColumnCount * frameWidth) >= frameWidth)
                   || (SpriteGenerator.GetNextPowerOfTwo(defaultColumnCount * Convert.ToInt32(frameWidth * 0.5))
                   - (defaultColumnCount * Convert.ToInt32(frameWidth * 0.5)) >= Convert.ToInt32(frameWidth * 0.5)))
                defaultColumnCount++;
            if (defaultColumnCount * frameWidth > 2056)
                throw new SpriteSizeException("Sprite sheet width greater than 2056!!");
            return defaultColumnCount;
        }

        private static int GetNextPowerOfTwo(int value)
        {
            Debug.Assert(value > 0);
            var computedValue = 1;
            while (value > computedValue)
                computedValue *= 2;
            return computedValue;
        }
    }
}