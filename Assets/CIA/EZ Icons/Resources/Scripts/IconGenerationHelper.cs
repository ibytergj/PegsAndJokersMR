using UnityEngine;

namespace CIA {

    public class IconGenerationHelper {

        // Adds a background behind the original texture with rounded corners

        public static Texture2D AddBackground(Texture2D originalTexture, Color backgroundColor, float borderRadius, float borderOffset, bool isMaskOnly) {
            // If mask is on, return the original texture
            if (isMaskOnly) {
                return originalTexture;
            }

            // Create a new texture for the background
            Texture2D backgroundTexture = new Texture2D(originalTexture.width, originalTexture.height, TextureFormat.ARGB32, false);
            Color[] originalPixels = originalTexture.GetPixels();
            Color[] backgroundPixels = new Color[originalPixels.Length];

            // Fill the background with the background color
            for (int y = 0; y < backgroundTexture.height; y++) {
                for (int x = 0; x < backgroundTexture.width; x++) {
                    int index = y * backgroundTexture.width + x;

                    // Calculate the distance to the corners for the rounded border
                    float distToCorner = CalculateDistanceToRoundedCorner(x, y, backgroundTexture.width, backgroundTexture.height, borderRadius - borderOffset);

                    // If we're within the rounded corner bounds, set the pixel to the background color
                    if (distToCorner <= 1f) {
                        backgroundPixels[index] = backgroundColor;
                    } else {
                        // Outside the rounded area, make it clear (transparent)
                        backgroundPixels[index] = Color.clear;
                    }
                }
            }

            backgroundTexture.SetPixels(backgroundPixels);

            // Now apply the original texture on top of the background
            for (int y = 0; y < backgroundTexture.height; y++) {
                for (int x = 0; x < backgroundTexture.width; x++) {
                    int index = y * backgroundTexture.width + x;
                    Color originalPixel = originalPixels[index];

                    // If the original pixel is not transparent, overwrite the background with the original texture's color
                    if (originalPixel.a > 0) {
                        backgroundPixels[index] = originalPixel;
                    }
                }
            }

            // Set the pixels with the new background and the icon on top
            backgroundTexture.SetPixels(backgroundPixels);
            backgroundTexture.Apply();

            return backgroundTexture;
        }

        public static Texture2D AddBorder(Texture2D originalTexture, Color borderColor, float borderRadius, float borderThickness, float borderOffset, bool isMaskOnly) {
            // If mask is on, return the original texture
            if (isMaskOnly) {
                return originalTexture;
            }

            Texture2D borderedTexture = new Texture2D(originalTexture.width, originalTexture.height, TextureFormat.ARGB32, false);
            Color[] originalPixels = originalTexture.GetPixels();
            Color[] borderedPixels = new Color[originalPixels.Length];

            for (int y = 0; y < borderedTexture.height; y++) {
                for (int x = 0; x < borderedTexture.width; x++) {
                    int index = y * borderedTexture.width + x;

                    // Calculate distance from corners, accounting for border radius and offset
                    float distToCorner = CalculateDistanceToRoundedCorner(x, y, borderedTexture.width, borderedTexture.height, borderRadius - borderOffset);

                    // Determine if the pixel is within the border range
                    if (distToCorner > 1f && distToCorner <= borderThickness + 1f) {
                        // Add border color
                        borderedPixels[index] = borderColor;
                    } else {
                        // Retain original pixels
                        borderedPixels[index] = originalPixels[index];
                    }
                }
            }

            borderedTexture.SetPixels(borderedPixels);
            borderedTexture.Apply();
            return borderedTexture;
        }



        /// Adds a smooth rounded border with consistent color and width

        public static Texture2D AddRoundedBorder2(Texture2D original, float radius, float borderWidth, float offset, Color borderColor) {
            int w = original.width;
            int h = original.height;
            Texture2D borderedTexture = new Texture2D(w, h, TextureFormat.ARGB32, false);
            Color[] originalPixels = original.GetPixels();
            Color[] borderedPixels = new Color[originalPixels.Length];

            // Define corner centers (moved inward by radius)
            Vector2 topLeft = new Vector2(offset + radius, offset + radius);
            Vector2 topRight = new Vector2(w - offset - radius - 1, offset + radius);
            Vector2 bottomLeft = new Vector2(offset + radius, h - offset - radius - 1);
            Vector2 bottomRight = new Vector2(w - offset - radius - 1, h - offset - radius - 1);

            // Define inner corner centers
            Vector2 innerTopLeft = new Vector2(offset + radius + borderWidth, offset + radius + borderWidth);
            Vector2 innerTopRight = new Vector2(w - offset - radius - 1 - borderWidth, offset + radius + borderWidth);
            Vector2 innerBottomLeft = new Vector2(offset + radius + borderWidth, h - offset - radius - 1 - borderWidth);
            Vector2 innerBottomRight = new Vector2(w - offset - radius - 1 - borderWidth, h - offset - radius - 1 - borderWidth);

            // Anti-aliasing width (smaller value = crisper edges)
            float aaWidth = 0.75f;

            // Loop through all pixels
            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {
                    Vector2 pixelPos = new Vector2(x, y);
                    float borderAlpha = 0f;

                    // Calculate distances from edges
                    float distanceFromLeft = x - offset;
                    float distanceFromRight = w - x - 1 - offset;
                    float distanceFromTop = y - offset;
                    float distanceFromBottom = h - y - 1 - offset;

                    // Check if we're in any corner region
                    if (x <= topLeft.x && y <= topLeft.y) // Top Left
{
                        float outerDist = Vector2.Distance(pixelPos, topLeft) - radius;
                        float innerDist = Vector2.Distance(pixelPos, topLeft) - (radius - borderWidth);
                        borderAlpha = GetSmoothBorderAlpha(innerDist, outerDist, aaWidth);
                    } else if (x >= topRight.x && y <= topRight.y) // Top Right
                      {
                        float outerDist = Vector2.Distance(pixelPos, topRight) - radius;
                        float innerDist = Vector2.Distance(pixelPos, topRight) - (radius - borderWidth);
                        borderAlpha = GetSmoothBorderAlpha(innerDist, outerDist, aaWidth);
                    } else if (x <= bottomLeft.x && y >= bottomLeft.y) // Bottom Left
                      {
                        float outerDist = Vector2.Distance(pixelPos, bottomLeft) - radius;
                        float innerDist = Vector2.Distance(pixelPos, bottomLeft) - (radius - borderWidth);
                        borderAlpha = GetSmoothBorderAlpha(innerDist, outerDist, aaWidth);
                    } else if (x >= bottomRight.x && y >= bottomRight.y) // Bottom Right
                      {
                        float outerDist = Vector2.Distance(pixelPos, bottomRight) - radius;
                        float innerDist = Vector2.Distance(pixelPos, bottomRight) - (radius - borderWidth);
                        borderAlpha = GetSmoothBorderAlpha(innerDist, outerDist, aaWidth);
                    } else // Straight sections
                      {
                        float minDistance = Mathf.Min(
                            distanceFromLeft,
                            distanceFromRight,
                            distanceFromTop,
                            distanceFromBottom
                        );

                        // Calculate the inner distance for straight sections
                        float innerDist = minDistance - borderWidth;

                        borderAlpha = GetSmoothBorderAlpha(innerDist, minDistance, aaWidth);
                    }



                    // Blend the border with the original pixel
                    Color originalColor = originalPixels[y * w + x];
                    if (originalColor.a == 0) {
                        Color borderColorWithAlpha = borderColor;
                        borderColorWithAlpha.a *= borderAlpha;
                        borderedPixels[y * w + x] = borderColorWithAlpha;
                    } else {
                        borderedPixels[y * w + x] = originalColor;
                    }
                }
            }

            borderedTexture.SetPixels(borderedPixels);
            borderedTexture.Apply();
            return borderedTexture;
        }

        public static Texture2D AddRoundedBorder(Texture2D original, float radius, float borderWidth, float offset, Color borderColor) {
            int w = original.width;
            int h = original.height;
            Texture2D borderedTexture = new Texture2D(w, h, TextureFormat.ARGB32, false);
            Color[] originalPixels = original.GetPixels();
            Color[] borderedPixels = new Color[originalPixels.Length];

            // Define corner centers (moved inward by radius)
            Vector2 topLeft = new Vector2(offset + radius, offset + radius);
            Vector2 topRight = new Vector2(w - offset - radius - 1, offset + radius);
            Vector2 bottomLeft = new Vector2(offset + radius, h - offset - radius - 1);
            Vector2 bottomRight = new Vector2(w - offset - radius - 1, h - offset - radius - 1);

            // Define inner corner centers
            Vector2 innerTopLeft = new Vector2(offset + radius + borderWidth, offset + radius + borderWidth);
            Vector2 innerTopRight = new Vector2(w - offset - radius - 1 - borderWidth, offset + radius + borderWidth);
            Vector2 innerBottomLeft = new Vector2(offset + radius + borderWidth, h - offset - radius - 1 - borderWidth);
            Vector2 innerBottomRight = new Vector2(w - offset - radius - 1 - borderWidth, h - offset - radius - 1 - borderWidth);

            // Anti-aliasing width (smaller value = crisper edges)
            float aaWidth = 0.75f;

            // Loop through all pixels
            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {
                    Vector2 pixelPos = new Vector2(x, y);
                    float borderAlpha = 0f;

                    // Calculate distances from edges
                    float distanceFromLeft = x - offset;
                    float distanceFromRight = w - x - 1 - offset;
                    float distanceFromTop = y - offset;
                    float distanceFromBottom = h - y - 1 - offset;

                    // Check if we're in any corner region
                    if (x <= topLeft.x && y <= topLeft.y) // Top Left
                    {
                        float outerDist = Vector2.Distance(pixelPos, topLeft) - radius;
                        float innerDist = Vector2.Distance(pixelPos, innerTopLeft) - (radius - borderWidth);
                        borderAlpha = GetSmoothBorderAlpha(innerDist, outerDist, aaWidth);
                    } else if (x >= topRight.x && y <= topRight.y) // Top Right
                      {
                        float outerDist = Vector2.Distance(pixelPos, topRight) - radius;
                        float innerDist = Vector2.Distance(pixelPos, innerTopRight) - (radius - borderWidth);
                        borderAlpha = GetSmoothBorderAlpha(innerDist, outerDist, aaWidth);
                    } else if (x <= bottomLeft.x && y >= bottomLeft.y) // Bottom Left
                      {
                        float outerDist = Vector2.Distance(pixelPos, bottomLeft) - radius;
                        float innerDist = Vector2.Distance(pixelPos, innerBottomLeft) - (radius - borderWidth);
                        borderAlpha = GetSmoothBorderAlpha(innerDist, outerDist, aaWidth);
                    } else if (x >= bottomRight.x && y >= bottomRight.y) // Bottom Right
                      {
                        float outerDist = Vector2.Distance(pixelPos, bottomRight) - radius;
                        float innerDist = Vector2.Distance(pixelPos, innerBottomRight) - (radius - borderWidth);
                        borderAlpha = GetSmoothBorderAlpha(innerDist, outerDist, aaWidth);
                    } else // Straight sections
                      {
                        float minDistance = Mathf.Min(
                            distanceFromLeft,
                            distanceFromRight,
                            distanceFromTop,
                            distanceFromBottom
                        );

                        if (minDistance >= 0) {
                            float innerDist = minDistance - borderWidth;
                            borderAlpha = GetSmoothBorderAlpha(innerDist, minDistance, aaWidth);
                        }
                    }


                    // Blend the border with the original pixel
                    Color originalColor = originalPixels[y * w + x];
                    if (originalColor.a == 0) {
                        Color borderColorWithAlpha = borderColor;
                        borderColorWithAlpha.a *= borderAlpha;
                        borderedPixels[y * w + x] = borderColorWithAlpha;
                    } else {
                        borderedPixels[y * w + x] = originalColor;
                    }
                }
            }

            borderedTexture.SetPixels(borderedPixels);
            borderedTexture.Apply();
            return borderedTexture;
        }

        private static float GetSmoothBorderAlpha(float innerDist, float outerDist, float aaWidth) {
            if (innerDist <= 0) return 1f;
            if (outerDist >= 0) return 0f;

            // Use linear interpolation with a narrow transition zone
            float t = Mathf.Clamp01(-outerDist / aaWidth);
            return t;
        }


        public static Texture2D AddRoundedBorder_Crisp(Texture2D original, float radius, float borderWidth, float offset, Color borderColor) {
            int w = original.width;
            int h = original.height;
            Texture2D borderedTexture = new Texture2D(w, h, TextureFormat.ARGB32, false);
            Color[] originalPixels = original.GetPixels();
            Color[] borderedPixels = new Color[originalPixels.Length];

            // Define corner centers (moved inward by radius)
            Vector2 topLeft = new Vector2(offset + radius, offset + radius);
            Vector2 topRight = new Vector2(w - offset - radius - 1, offset + radius);
            Vector2 bottomLeft = new Vector2(offset + radius, h - offset - radius - 1);
            Vector2 bottomRight = new Vector2(w - offset - radius - 1, h - offset - radius - 1);

            // Loop through all pixels
            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {
                    Vector2 pixelPos = new Vector2(x, y);
                    bool isBorderPixel = false;

                    // Calculate distances from edges
                    float distanceFromLeft = x - offset;
                    float distanceFromRight = w - x - 1 - offset;
                    float distanceFromTop = y - offset;
                    float distanceFromBottom = h - y - 1 - offset;

                    // Check if we're in any corner region
                    if (x <= topLeft.x && y <= topLeft.y) // Top Left
                    {
                        float dist = Vector2.Distance(pixelPos, topLeft);
                        isBorderPixel = dist <= radius && dist > radius - borderWidth;
                    } else if (x >= topRight.x && y <= topRight.y) // Top Right
                      {
                        float dist = Vector2.Distance(pixelPos, topRight);
                        isBorderPixel = dist <= radius && dist > radius - borderWidth;
                    } else if (x <= bottomLeft.x && y >= bottomLeft.y) // Bottom Left
                      {
                        float dist = Vector2.Distance(pixelPos, bottomLeft);
                        isBorderPixel = dist <= radius && dist > radius - borderWidth;
                    } else if (x >= bottomRight.x && y >= bottomRight.y) // Bottom Right
                      {
                        float dist = Vector2.Distance(pixelPos, bottomRight);
                        isBorderPixel = dist <= radius && dist > radius - borderWidth;
                    } else // Straight sections
                      {
                        // Check if pixel lies within border width along any straight edge
                        isBorderPixel = (distanceFromLeft < borderWidth ||
                                         distanceFromRight < borderWidth ||
                                         distanceFromTop < borderWidth ||
                                         distanceFromBottom < borderWidth) &&
                                        distanceFromLeft >= 0 &&
                                        distanceFromRight >= 0 &&
                                        distanceFromTop >= 0 &&
                                        distanceFromBottom >= 0;
                    }

                    // Only set border color if the original pixel is transparent
                    if (isBorderPixel && originalPixels[y * w + x].a == 0) {
                        borderedPixels[y * w + x] = borderColor;
                    } else {
                        borderedPixels[y * w + x] = originalPixels[y * w + x];
                    }
                }
            }

            borderedTexture.SetPixels(borderedPixels);
            borderedTexture.Apply();
            return borderedTexture;
        }



        public static Texture2D AddRoundedBorder_Jagged(Texture2D original, float radius, float borderWidth, float offset, Color borderColor) {
            int w = original.width;
            int h = original.height;
            Texture2D borderedTexture = new Texture2D(w, h, TextureFormat.ARGB32, false);
            Color[] originalPixels = original.GetPixels();
            Color[] borderedPixels = new Color[originalPixels.Length];

            // Define corner centers (moved inward by radius)
            Vector2 topLeft = new Vector2(offset + radius, offset + radius);
            Vector2 topRight = new Vector2(w - offset - radius - 1, offset + radius);
            Vector2 bottomLeft = new Vector2(offset + radius, h - offset - radius - 1);
            Vector2 bottomRight = new Vector2(w - offset - radius - 1, h - offset - radius - 1);

            // Loop through all pixels
            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {
                    Vector2 pixelPos = new Vector2(x, y);
                    bool isBorderPixel = false;

                    // Calculate distances from edges
                    float distanceFromLeft = x - offset;
                    float distanceFromRight = w - x - 1 - offset;
                    float distanceFromTop = y - offset;
                    float distanceFromBottom = h - y - 1 - offset;

                    // Check if we're in any corner region
                    if (x <= topLeft.x && y <= topLeft.y) // Top Left
                    {
                        float dist = Vector2.Distance(pixelPos, topLeft);
                        isBorderPixel = dist <= radius && dist > radius - borderWidth;
                    } else if (x >= topRight.x && y <= topRight.y) // Top Right
                      {
                        float dist = Vector2.Distance(pixelPos, topRight);
                        isBorderPixel = dist <= radius && dist > radius - borderWidth;
                    } else if (x <= bottomLeft.x && y >= bottomLeft.y) // Bottom Left
                      {
                        float dist = Vector2.Distance(pixelPos, bottomLeft);
                        isBorderPixel = dist <= radius && dist > radius - borderWidth;
                    } else if (x >= bottomRight.x && y >= bottomRight.y) // Bottom Right
                      {
                        float dist = Vector2.Distance(pixelPos, bottomRight);
                        isBorderPixel = dist <= radius && dist > radius - borderWidth;
                    } else // Straight sections
                      {
                        isBorderPixel = (distanceFromLeft < borderWidth ||
                                        distanceFromRight < borderWidth ||
                                        distanceFromTop < borderWidth ||
                                        distanceFromBottom < borderWidth) &&
                                       distanceFromLeft >= 0 &&
                                       distanceFromRight >= 0 &&
                                       distanceFromTop >= 0 &&
                                       distanceFromBottom >= 0;
                    }

                    // Only set border color if the original pixel is transparent
                    if (isBorderPixel && originalPixels[y * w + x].a == 0) {
                        borderedPixels[y * w + x] = borderColor;
                    } else {
                        borderedPixels[y * w + x] = originalPixels[y * w + x];
                    }
                }
            }

            borderedTexture.SetPixels(borderedPixels);
            borderedTexture.Apply();
            return borderedTexture;
        }


        public static Texture2D AddRoundedBorderGood(Texture2D original, float radius, float borderWidth, float offset, Color borderColor) {
            int w = original.width;
            int h = original.height;
            Texture2D borderedTexture = new Texture2D(w, h, TextureFormat.ARGB32, false);
            Color[] originalPixels = original.GetPixels();
            Color[] borderedPixels = new Color[originalPixels.Length];

            // Define corner centers (moved inward by radius)
            Vector2 topLeft = new Vector2(offset + radius, offset + radius);
            Vector2 topRight = new Vector2(w - offset - radius - 1, offset + radius);
            Vector2 bottomLeft = new Vector2(offset + radius, h - offset - radius - 1);
            Vector2 bottomRight = new Vector2(w - offset - radius - 1, h - offset - radius - 1);

            // Loop through all pixels
            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {
                    Vector2 pixelPos = new Vector2(x, y);
                    bool isBorderPixel = false;

                    // Calculate distances from edges
                    float distanceFromLeft = x - offset;
                    float distanceFromRight = w - x - 1 - offset;
                    float distanceFromTop = y - offset;
                    float distanceFromBottom = h - y - 1 - offset;

                    // Check if we're in any corner region
                    if (x <= topLeft.x && y <= topLeft.y) // Top Left
                    {
                        float dist = Vector2.Distance(pixelPos, topLeft);
                        isBorderPixel = dist <= radius && dist > radius - borderWidth;
                    } else if (x >= topRight.x && y <= topRight.y) // Top Right
                      {
                        float dist = Vector2.Distance(pixelPos, topRight);
                        isBorderPixel = dist <= radius && dist > radius - borderWidth;
                    } else if (x <= bottomLeft.x && y >= bottomLeft.y) // Bottom Left
                      {
                        float dist = Vector2.Distance(pixelPos, bottomLeft);
                        isBorderPixel = dist <= radius && dist > radius - borderWidth;
                    } else if (x >= bottomRight.x && y >= bottomRight.y) // Bottom Right
                      {
                        float dist = Vector2.Distance(pixelPos, bottomRight);
                        isBorderPixel = dist <= radius && dist > radius - borderWidth;
                    } else // Straight sections
                      {
                        isBorderPixel = (distanceFromLeft < borderWidth ||
                                        distanceFromRight < borderWidth ||
                                        distanceFromTop < borderWidth ||
                                        distanceFromBottom < borderWidth) &&
                                       distanceFromLeft >= 0 &&
                                       distanceFromRight >= 0 &&
                                       distanceFromTop >= 0 &&
                                       distanceFromBottom >= 0;
                    }

                    // Only set border color if the original pixel is transparent
                    if (isBorderPixel && originalPixels[y * w + x].a == 0) {
                        borderedPixels[y * w + x] = borderColor;
                    } else {
                        borderedPixels[y * w + x] = originalPixels[y * w + x];
                    }
                }
            }

            borderedTexture.SetPixels(borderedPixels);
            borderedTexture.Apply();
            return borderedTexture;
        }

        public static Texture2D AddRoundedBorder_Inward(Texture2D original, float radius, float borderWidth, float offset, Color borderColor) {
            int w = original.width;
            int h = original.height;
            Texture2D borderedTexture = new Texture2D(w, h, TextureFormat.ARGB32, false);
            Color[] originalPixels = original.GetPixels();
            Color[] borderedPixels = new Color[originalPixels.Length];

            // Loop through all pixels
            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {
                    bool isBorderPixel = false;

                    // Calculate the distance to the nearest edge
                    float edgeX = (x < w / 2) ? offset : (w - 1 - offset);
                    float edgeY = (y < h / 2) ? offset : (h - 1 - offset);

                    // Are we in a corner region?
                    bool isInCornerRegion = (Mathf.Abs(x - edgeX) < radius) && (Mathf.Abs(y - edgeY) < radius);

                    if (isInCornerRegion) {
                        // For corner regions, calculate distance from the corner
                        float distanceFromCorner = Vector2.Distance(new Vector2(x, y), new Vector2(edgeX, edgeY));
                        float innerRadius = radius - borderWidth;

                        // Border pixel if between inner and outer radius
                        isBorderPixel = distanceFromCorner <= radius && distanceFromCorner >= innerRadius;
                    } else {
                        // For straight sections, check if within border width
                        float distanceFromLeft = Mathf.Abs(x - offset);
                        float distanceFromRight = Mathf.Abs(x - (w - 1 - offset));
                        float distanceFromTop = Mathf.Abs(y - offset);
                        float distanceFromBottom = Mathf.Abs(y - (h - 1 - offset));

                        float minDistance = Mathf.Min(
                            distanceFromLeft,
                            distanceFromRight,
                            distanceFromTop,
                            distanceFromBottom
                        );

                        isBorderPixel = minDistance <= borderWidth;
                    }

                    borderedPixels[y * w + x] = isBorderPixel ? borderColor : originalPixels[y * w + x];
                }
            }

            borderedTexture.SetPixels(borderedPixels);
            borderedTexture.Apply();
            return borderedTexture;
        }



        public static Texture2D AddBackgroundFill(
            Texture2D original,
            float radius,
            float width,
            float offset,
            Color backgroundColor,
            bool isMaskOnly = false) {
            if (isMaskOnly) {
                return original;
            }
            int w = original.width;
            int h = original.height;
            Texture2D backgroundTexture = new Texture2D(w, h, TextureFormat.ARGB32, false);
            Color[] backgroundPixels = new Color[w * h];

            // Define corner centers (adjusted inward by radius + offset)
            Vector2 topLeft = new Vector2(offset + radius, offset + radius);
            Vector2 topRight = new Vector2(w - offset - radius - 1, offset + radius);
            Vector2 bottomLeft = new Vector2(offset + radius, h - offset - radius - 1);
            Vector2 bottomRight = new Vector2(w - offset - radius - 1, h - offset - radius - 1);

            float innerRadius = radius - width;

            // Fill background
            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {
                    Vector2 pixelPos = new Vector2(x, y);
                    bool isBackgroundPixel = false;

                    // Calculate distances for straight edges
                    float distanceFromLeft = x - (offset + width);
                    float distanceFromRight = w - x - 1 - (offset + width);
                    float distanceFromTop = y - (offset + width);
                    float distanceFromBottom = h - y - 1 - (offset + width);

                    // Corner calculations
                    if (x <= topLeft.x && y <= topLeft.y) { // Top Left
                        float dist = Vector2.Distance(pixelPos, topLeft);
                        isBackgroundPixel = dist <= innerRadius;
                    } else if (x >= topRight.x && y <= topRight.y) { // Top Right
                        float dist = Vector2.Distance(pixelPos, topRight);
                        isBackgroundPixel = dist <= innerRadius;
                    } else if (x <= bottomLeft.x && y >= bottomLeft.y) { // Bottom Left
                        float dist = Vector2.Distance(pixelPos, bottomLeft);
                        isBackgroundPixel = dist <= innerRadius;
                    } else if (x >= bottomRight.x && y >= bottomRight.y) { // Bottom Right
                        float dist = Vector2.Distance(pixelPos, bottomRight);
                        isBackgroundPixel = dist <= innerRadius;
                    } else { // Straight sections within the rounded rectangle
                        isBackgroundPixel = distanceFromLeft >= 0 &&
                                            distanceFromRight >= 0 &&
                                            distanceFromTop >= 0 &&
                                            distanceFromBottom >= 0;
                    }

                    // Set background pixels
                    if (isBackgroundPixel) {
                        backgroundPixels[y * w + x] = isMaskOnly
                            ? new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, 1f)
                            : backgroundColor;
                    }
                }
            }

            // Overlay the original texture on top of the background
            Color[] originalPixels = original.GetPixels();
            for (int i = 0; i < backgroundPixels.Length; i++) {
                if (originalPixels[i].a > 0) {
                    backgroundPixels[i] = originalPixels[i];
                }
            }

            backgroundTexture.SetPixels(backgroundPixels);
            backgroundTexture.Apply();
            return backgroundTexture;
        }





        /// <summary>
        /// Calculates the smoothed distance to the rounded corner
        /// </summary>
        private static float CalculateDistanceToRoundedCorner(int x, int y, int width, int height, float borderRadius) {
            // Determine corner quadrants
            bool topLeft = x < borderRadius && y < borderRadius;
            bool topRight = x >= width - borderRadius && y < borderRadius;
            bool bottomLeft = x < borderRadius && y >= height - borderRadius;
            bool bottomRight = x >= width - borderRadius && y >= height - borderRadius;

            if (topLeft || topRight || bottomLeft || bottomRight) {
                float cornerCenterX = topLeft || bottomLeft ? borderRadius : width - borderRadius;
                float cornerCenterY = topLeft || topRight ? borderRadius : height - borderRadius;

                float dx = x - cornerCenterX;
                float dy = y - cornerCenterY;
                float distance = Mathf.Sqrt(dx * dx + dy * dy);

                // Smooth falloff
                float smoothRadius = borderRadius + 1f;
                return distance / smoothRadius;
            }

            // Inside the main rectangle or edge regions
            return 0f;
        }


        public static Bounds CalculateBounds(GameObject obj) {
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) return new Bounds(obj.transform.position, Vector3.zero);

            Bounds bounds = renderers[0].bounds;
            foreach (Renderer renderer in renderers) {
                bounds.Encapsulate(renderer.bounds);
            }
            return bounds;
        }


        public static void EnsureSavePathExists(string path) {
            try {
                // Check if path is null or empty
                if (string.IsNullOrWhiteSpace(path)) {
                    Debug.LogError("Save path is null or empty!");
                    return;
                }

                // Log full path details
                Debug.Log($"Attempting to create directory at: {path}");
                Debug.Log($"Path exists check: {System.IO.Directory.Exists(path)}");
                Debug.Log($"Full path is absolute: {System.IO.Path.IsPathRooted(path)}");

                // Attempt to create directory with more comprehensive error handling
                if (!System.IO.Directory.Exists(path)) {
                    try {
                        System.IO.Directory.CreateDirectory(path);
                        Debug.Log($"Directory successfully created at: {path}");
                    } catch (System.UnauthorizedAccessException uaEx) {
                        Debug.LogError($"Unauthorized access when creating directory: {uaEx.Message}");
                    } catch (System.IO.IOException ioEx) {
                        Debug.LogError($"IO Exception when creating directory: {ioEx.Message}");
                    } catch (System.Exception Ex) {
                        Debug.LogError($"Unexpected error creating directory: {Ex.Message}");
                    }
                }

                // Double-check directory existence after creation attempt
                if (!System.IO.Directory.Exists(path)) {
                    Debug.LogError($"Failed to create directory at: {path}");
                } else {
                    Debug.Log($"Confirmed directory exists at: {path}");
                }
            } catch (System.Exception ex) {
                Debug.LogError($"Critical error in EnsureSavePathExists: {ex.Message}");
            }
        }


        public static Texture2D ConvertToMask(Texture2D originalTexture, Color maskColor) {
            Texture2D maskTexture = new Texture2D(originalTexture.width, originalTexture.height, TextureFormat.ARGB32, false);
            Color[] pixels = originalTexture.GetPixels();
            Color[] maskPixels = new Color[pixels.Length];

            for (int i = 0; i < pixels.Length; i++) {
                // Preserve the original alpha, but use the mask color
                maskPixels[i] = new Color(maskColor.r, maskColor.g, maskColor.b, pixels[i].a);
            }

            maskTexture.SetPixels(maskPixels);
            maskTexture.Apply();
            return maskTexture;
        }

    }


    public static class CameraHelper {
        public static void CleanupTempCameras() {
            // Find and destroy all objects named "TempCamera"
            GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            foreach (GameObject obj in allObjects) {
                if (obj.name == "TempCamera") {
                    Object.DestroyImmediate(obj);
                }
            }
        }
    }

}