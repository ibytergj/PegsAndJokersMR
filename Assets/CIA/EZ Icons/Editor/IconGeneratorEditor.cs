using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;
using Object = UnityEngine.Object;
using System.IO;

namespace CIA {
    public class IconGeneratorEditor : EditorWindow {

        // UXML ELEMENTS
        private Button generateButton;
        private ObjectField prefabField;
        private VisualElement imagePreviewPane;
        private VisualElement imagePreviewBg;
        private Vector3Field rotationField;
        private Slider scaleSlider;
        private SliderInt iconSizeSlider;
        private FloatField iconSizeField;
        private Slider cornerRadiusSlider;
        private Toggle borderToggle;
        private VisualElement borderPane;
        private Slider borderWidthSlider;
        private Slider borderOffsetSlider;
        private ColorField borderColorField;
        private Toggle ornamentalToggle;
        private Toggle fillToggle;
        private VisualElement fillPane;
        private ColorField fillColorField;
        private VisualElement lightPane;
        private Toggle lightToggle;
        private Slider lightIntensitySlider;
        private ColorField lightColorField;
        private TextField savePathField;
        private Button browseButton;
        private TextField prefabNameField;
        private Toggle usePrefabNameToggle;
        private VisualElement customFileNamePane;
        private TextField customFileNameField;
        private TextField fullPathField;
        private Toggle maskToggle;
        private VisualElement maskPane;
        private ColorField maskColorField;
        private Label noPrefabLabel;
        private Toggle debugToggle;

        private Toggle customIconSizeToggle;
        private VisualElement customIconSizePane;
        private Vector2IntField customIconSizeField;

        private bool debugMode = false;
        private GameObject prefab;
        private Vector2Int iconDimensions = new Vector2Int(256, 256);
        //private bool uniformIconDimensions = true;
        private Vector3 prefabScale = Vector3.one;
        //private bool uniformScale = true;
        private Vector3 prefabRotation = Vector3.zero;
        private string savePath = "Assets/Icons";
        private string fileName = "Icon";
        private bool usePrefabName = true;

        private bool isMaskOnly = false;
        private Color maskColor = Color.white;

        private bool addBorder = true;
        private float radius = 10f;
        private float borderWidth = 0.1f;
        private float borderOffset = 0f;
        private Color borderColor = Color.white;
        private bool ornamentalCorners = false;

        private bool addBackground = false; // To track whether the background should be added
        private Color backgroundColor = Color.white; // To track the selected background color


        private Texture2D previewTexture;
        private Camera tempCamera;
        private GameObject tempCameraObj;

        private bool compatibleWithCozyWeather = false;
        private GameObject cozyWeatherSphere;


        // LIGHTS
        bool addLight = false;
        GameObject tempLight;
        Light lightComponent;
        Color lightColor = Color.white;
        float lightIntensity = 1f;
        LightShadows lightShadows = LightShadows.None;


        // UI STUFF
        VisualElement root;

        [MenuItem("Tools/CIA Devs/EZ Icons")]
        public static void OpenEditorWindow() {
            var window = GetWindow<IconGeneratorEditor>("CIA EZ Icons");
            window.Show();
        }

        private void OnEnable() {
            HandleCozyWeatherSphere(false);
        }

        private void OnDisable() {
            // Destroy the camera when the tool is closed
            if (tempCameraObj != null) {
                DestroyImmediate(tempCameraObj);
            }
            HandleCozyWeatherSphere(true);
            CleanUp();
        }

        private void CleanUp() {

            CameraHelper.CleanupTempCameras();
        }


        private void CreateGUI() {

            root = rootVisualElement;
            string guid = "74951704c5f2f8a4e98314db2bafb4a1"; // UXML GUID
            string path = AssetDatabase.GUIDToAssetPath(guid);
            //var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Assets/CIA/EZ Icons/Resources/UI Documents/IconGeneratorWindow.uxml); // STATIC PATH
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path); // GUID PATH

            VisualElement tree = visualTree.Instantiate();
            root.Add(tree);

            RegisterParameters();
            AssignCallbacks();
            SetValues();
        }

        private void OnGUI() {

        }



        private void RefreshPreview() {
            if (prefab != null) {
                noPrefabLabel.style.display = DisplayStyle.None;
                previewTexture = GenerateIcon(true);
                generateButton.SetEnabled(true);
                generateButton.RemoveFromClassList("buttonLarge-animated-noHover:hover");
            } else {
                noPrefabLabel.style.display = DisplayStyle.Flex;
                generateButton.SetEnabled(false);
                generateButton.AddToClassList("buttonLarge-animated-noHover:hover");
            }
        }


        private void RegisterParameters() {

            prefabField = root.Q<ObjectField>("prefab-field");
            generateButton = root.Q<Button>("generate-button");
            imagePreviewPane = root.Q<VisualElement>("image-preview");
            imagePreviewBg = root.Q<VisualElement>("imagePreview-bg");
            rotationField = root.Q<Vector3Field>("rotation-field");
            scaleSlider = root.Q<Slider>("scale-slider");
            iconSizeSlider = root.Q<SliderInt>("iconSize-slider");
            iconSizeField = root.Q<FloatField>("iconSize-field");
            cornerRadiusSlider = root.Q<Slider>("cornerRadius-slider");
            borderToggle = root.Q<Toggle>("border-toggle");
            borderPane = root.Q<VisualElement>("borderSettings-pane");
            borderWidthSlider = root.Q<Slider>("borderWidth-slider");
            borderOffsetSlider = root.Q<Slider>("borderOffset-slider");
            //borderOffsetField = root.Q<FloatField>("borderOffset-field");
            borderColorField = root.Q<ColorField>("border-color");
            ornamentalToggle = root.Q<Toggle>("ornamental-toggle");
            fillToggle = root.Q<Toggle>("fill-toggle");
            fillPane = root.Q<VisualElement>("fill-pane");
            fillColorField = root.Q<ColorField>("fill-color");
            lightPane = root.Q<VisualElement>("light-pane");
            lightToggle = root.Q<Toggle>("light-toggle");
            lightIntensitySlider = root.Q<Slider>("lightIntensity-slider");
            lightColorField = root.Q<ColorField>("light-color");
            savePathField = root.Q<TextField>("savePath-field");
            browseButton = root.Q<Button>("browse-button");
            usePrefabNameToggle = root.Q<Toggle>("prefabName-toggle");
            prefabNameField = root.Q<TextField>("prefabName-field");
            customFileNamePane = root.Q<VisualElement>("customName-pane");
            customFileNameField = root.Q<TextField>("customName-field");
            fullPathField = root.Q<TextField>("fullPath-field");

            customIconSizeToggle = root.Q<Toggle>("iconCustom-toggle");
            customIconSizePane = root.Q<VisualElement>("iconCustomSize-pane");
            customIconSizeField = root.Q<Vector2IntField>("customIconSizeField");

            maskToggle = root.Q<Toggle>("mask-toggle");
            maskPane = root.Q<VisualElement>("mask-pane");
            maskColorField = root.Q<ColorField>("mask-color");

            noPrefabLabel = root.Q<Label>("noPrefab-label");

            debugToggle = root.Q<Toggle>("debug-toggle");

        }

        private void SetValues() {

            generateButton.SetEnabled(false);
            scaleSlider.value = prefabScale.x;

            // Set Icon Dimensions on Start
            int pw2 = Mathf.RoundToInt(Mathf.Pow(2, iconSizeSlider.value) * 16);
            Vector2Int newDimensions = new Vector2Int(pw2, pw2);
            iconDimensions = newDimensions;
            iconSizeField.value = pw2;
            customIconSizeField.value = newDimensions;

            HandleCustomIconModeChanged(customIconSizeToggle.value);

            //Set Default File Location
            UpdateFileLocationOnFolderLoad();
            UpdateIconName(usePrefabNameToggle.value);

            //radius = cornerRadiusSlider.value;
            radius = cornerRadiusSlider.value * MathF.Sqrt(iconDimensions.x * iconDimensions.y) * 0.5f;
            borderWidth = borderWidthSlider.value * Mathf.Sqrt(iconDimensions.x * iconDimensions.y) * 0.5f;

            RecalculateBorderOffset(borderOffsetSlider.value);
            borderColor = borderColorField.value;

            addBackground = fillToggle.value;
            RecalculateFill(fillColorField.value);


            addLight = lightToggle.value;
            lightIntensity = lightIntensitySlider.value;
            lightColor = lightColorField.value;

            UpdateMask(maskToggle.value);

            RefreshPreview();

        }


        private void AssignCallbacks() {

            prefabField.RegisterValueChangedCallback<Object>(PrefabUpdated);
            generateButton.RegisterCallback<MouseUpEvent>((evt) => GenerateButtonPressed());
            rotationField.RegisterValueChangedCallback<Vector3>(UpdateRotation);
            scaleSlider.RegisterValueChangedCallback<float>(UpdateScale);
            iconSizeSlider.RegisterValueChangedCallback<int>(UpdateIconSize);

            cornerRadiusSlider.RegisterValueChangedCallback<float>(UpdateCornerRadius);
            borderToggle.RegisterCallback<ChangeEvent<bool>>(borderTogglePressed);
            //borderPane = root.Q<VisualElement>("border-pane");
            borderWidthSlider.RegisterValueChangedCallback<float>(UpdateBorderWidth);
            borderOffsetSlider.RegisterValueChangedCallback<float>(UpdateBorderOffset);
            //borderOffsetField.RegisterValueChangedCallback<float>(UpdateBorderOffset);
            borderColorField.RegisterValueChangedCallback<Color>(UpdateBorderOffset);
            ornamentalToggle.RegisterCallback<ChangeEvent<bool>>(ornamentalTogglePressed);
            fillToggle.RegisterCallback<ChangeEvent<bool>>(fillTogglePressed);
            fillColorField.RegisterValueChangedCallback<Color>(UpdateFillColor);

            lightToggle.RegisterCallback<ChangeEvent<bool>>(lightTogglePressed);
            lightIntensitySlider.RegisterValueChangedCallback<float>(UpdateLightIntensity);
            lightColorField.RegisterValueChangedCallback<Color>(UpdateLightColor);

            browseButton.RegisterCallback<MouseUpEvent>((evt) => BrowseButtonPressed());

            usePrefabNameToggle.RegisterCallback<ChangeEvent<bool>>(usePrefabNameTogglePressed);
            customFileNameField.RegisterCallback<ChangeEvent<string>>(CustomFileNameUpdated);
            //savePathField.RegisterValueChangedCallback<string>(UpdatedSavePath);
            maskToggle.RegisterCallback<ChangeEvent<bool>>(MaskTogglePressed);
            maskColorField.RegisterValueChangedCallback<Color>(MaskColorUpdated);

            customIconSizeToggle.RegisterCallback<ChangeEvent<bool>>(customIconSizeTogglePressed);
            customIconSizeField.RegisterValueChangedCallback<Vector2Int>(UpdateIconCustomSize);

            debugToggle.RegisterCallback<ChangeEvent<bool>>(debugTogglePressed);
        }

        private void debugTogglePressed(ChangeEvent<bool> evt) {
            debugMode = evt.newValue;
        }

        private void MaskColorUpdated(ChangeEvent<Color> evt) {

            maskColor = evt.newValue;
            UpdatePreview();
        }

        private void MaskTogglePressed(ChangeEvent<bool> evt) {

            UpdateMask(evt.newValue);
            UpdatePreview();
        }

        private void UpdateMask(bool show) {

            isMaskOnly = show;
            if (show) {
                maskPane.style.display = DisplayStyle.Flex;
            } else {
                maskPane.style.display = DisplayStyle.None;
            }
        }

        private void BrowseButtonPressed() {

            // Determine the initial path
            string initialPath = string.IsNullOrEmpty(savePath)
                ? Application.dataPath // Default to Assets folder if savePath is not set
                : Application.dataPath + savePath.Replace("Assets", ""); // Convert Assets path to absolute path

            // Open folder panel with the initial path
            string path = EditorUtility.OpenFolderPanel("Select Save Path", initialPath, "");
            if (!string.IsNullOrEmpty(path)) {
                // Convert absolute path back to relative Unity Assets path
                savePath = "Assets" + path.Replace(Application.dataPath, "");
                UpdateFileLocationOnFolderLoad();
                //EditorUtility.SetDirty(this); // Mark the editor window as dirty
            }

        }


        private void UpdateFileLocationOnFolderLoad() {

            savePathField.value = savePath;
            UpdateFullFileInfo();

        }

        private void CustomFileNameUpdated(ChangeEvent<string> evt) {

            fileName = evt.newValue;
            UpdateFullFileInfo();
        }

        private void UpdateFullFileInfo() {

            fileName = customFileNameField.value;

            string filePath = $"{savePath}/{fileName}.png";
            if (usePrefabName) {
                if (prefab != null) {

                    filePath = $"{savePath}/{prefab.name}.png";
                } else {
                    filePath = "NO PREFAB LOADED";
                }
            }

            fullPathField.value = filePath;
        }


    private void UpdateBorderOffset(ChangeEvent<Color> evt) {

            borderColor = evt.newValue;
            UpdatePreview();
        }

        private void UpdateCornerRadius(ChangeEvent<float> evt) {

            
            RecalculateCornerRadius(evt.newValue);
            if (evt.newValue + borderOffsetSlider.value > 1) {
                borderOffsetSlider.value = 1 - evt.newValue;
            }
            UpdatePreview();
        }

        private void RecalculateCornerRadius(float value) {

            radius = value * MathF.Sqrt(iconDimensions.x * iconDimensions.y) * 0.5f;
            
        }

        private void UpdateBorderOffset(ChangeEvent<float> evt) {

            
            RecalculateBorderOffset(evt.newValue);
            if (evt.newValue + cornerRadiusSlider.value > 1) {
                cornerRadiusSlider.value = 1 - evt.newValue;
            }
            UpdatePreview();
        }

        private void RecalculateBorderOffset(float value) {

            borderOffset = value * Mathf.Sqrt(iconDimensions.x * iconDimensions.y) * 0.25f;
        }

        private void UpdateBorderWidth(ChangeEvent<float> evt) {

            RecalculateBorderWidth(evt.newValue);
            UpdatePreview();
        }

        private void RecalculateBorderWidth(float value) {

            borderWidth = value * Mathf.Sqrt(iconDimensions.x * iconDimensions.y) * 0.5f;

        }

        private void borderTogglePressed(ChangeEvent<bool> evt) {

            if (evt.newValue) {

                borderPane.style.display = DisplayStyle.Flex;

            } else {

                borderPane.style.display = DisplayStyle.None;
            }

            addBorder = evt.newValue;
            UpdatePreview();
        }

        private void ornamentalTogglePressed(ChangeEvent<bool> evt) {

            ornamentalCorners = evt.newValue;
            UpdatePreview();

        }

        private void customIconSizeTogglePressed(ChangeEvent<bool> evt) {

            HandleCustomIconModeChanged(evt.newValue);

        }

        private void HandleCustomIconModeChanged(bool isTrue) {

            if (isTrue) {
                
                iconSizeSlider.SetEnabled(false);
                customIconSizePane.style.display = DisplayStyle.Flex;
                iconDimensions = customIconSizeField.value;

            } else {

                iconSizeSlider.SetEnabled(true);
                customIconSizePane.style.display = DisplayStyle.None;
                int pw2 = Mathf.RoundToInt(Mathf.Pow(2, iconSizeSlider.value) * 16);
                Vector2Int newDimensions = new Vector2Int(pw2, pw2);
                iconDimensions = newDimensions;

            }
            UpdatePreview();
        }

        private void UpdateIconCustomSize(ChangeEvent<Vector2Int> evt) {

            iconDimensions = evt.newValue;
            UpdatePreview();
        }


        private void UpdateFillColor(ChangeEvent<Color> evt) {

            RecalculateFill(evt.newValue);
            UpdatePreview();
        }

        private void RecalculateFill(Color color) {

            backgroundColor = color;
        }

        private void fillTogglePressed(ChangeEvent<bool> evt) {

            if (evt.newValue) {
                fillPane.style.display = DisplayStyle.Flex;
            } else {
                fillPane.style.display = DisplayStyle.None;
            }

            addBackground = evt.newValue;
            UpdatePreview();
        }


        private void lightTogglePressed(ChangeEvent<bool> evt) {

            if (evt.newValue) {
                lightPane.style.display = DisplayStyle.Flex;
            } else {
                lightPane.style.display = DisplayStyle.None;
            }

            addLight = evt.newValue;
            UpdatePreview();
        }


        private void UpdateLightColor(ChangeEvent<Color> evt) {

            lightColor = evt.newValue;
            UpdatePreview();
        }

        private void UpdateLightIntensity(ChangeEvent<float> evt) {

            lightIntensity = evt.newValue;
            UpdatePreview();
        }


        private void usePrefabNameTogglePressed(ChangeEvent<bool> evt) {

            UpdateIconName(evt.newValue);
        }

        private void UpdateIconName(bool isUsingPrefabName) {

            if (isUsingPrefabName) {

                customFileNamePane.style.display = DisplayStyle.None;
            } else {
                customFileNamePane.style.display = DisplayStyle.Flex;
            }
            usePrefabName = isUsingPrefabName;
            fileName = customFileNameField.value;
            UpdateFullFileInfo();

        }


        private void UpdateIconSize(ChangeEvent<int> evt) {

            int pw2 = Mathf.RoundToInt(Mathf.Pow(2, evt.newValue) * 16);
            iconDimensions = new Vector2Int(pw2, pw2);
            iconSizeField.value = pw2;
            RecalculateCornerRadius(cornerRadiusSlider.value);
            RecalculateBorderWidth(borderWidthSlider.value);
            RecalculateBorderOffset(borderOffsetSlider.value);

            UpdatePreview();
        }

        private void UpdateScale(ChangeEvent<float> evt) {

            prefabScale = new Vector3(evt.newValue, evt.newValue, evt.newValue);
            UpdatePreview();
        }

        private void UpdateRotation(ChangeEvent<Vector3> evt) {

            prefabRotation = rotationField.value;
            UpdatePreview();
        }

        private void PrefabUpdated(ChangeEvent<Object> evt) {
            //HandleDebug("Prefab Updates");
            try {
                prefab = (GameObject)prefabField.value;
                prefabNameField.value = prefab.name;
                UpdateIconName(usePrefabNameToggle.value);
                
                UpdatePreview();
            } catch (Exception) {
                prefabNameField.value = "NO PREFAB LOADED";
                UpdateFullFileInfo();
                UpdatePreview();
            }
        }

        private void UpdatePreview() {

            if (prefab == null) {
                noPrefabLabel.style.display = DisplayStyle.Flex;
                generateButton.SetEnabled(false);
                generateButton.AddToClassList("buttonLarge-animated-noHover:hover");
                imagePreviewPane.style.backgroundImage = null;
                //imagePreviewPane.style.display = DisplayStyle.None;
                return;
            }
            noPrefabLabel.style.display = DisplayStyle.None;
            previewTexture = GenerateIcon(true);
            generateButton.SetEnabled(true);
            generateButton.RemoveFromClassList("buttonLarge-animated-noHover:hover");
            imagePreviewPane.style.backgroundImage = previewTexture;
        }



        private void GenerateButtonPressed() {

            if (VerifySavePath()) {
                GenerateIcon(false);
            }
        }

        private Texture2D GenerateIcon(bool isPreview) {
            try {

                // Create a temporary camera
                tempCameraObj = new GameObject("CIA TempCamera");
                tempCamera = tempCameraObj.AddComponent<Camera>();
                tempCamera.clearFlags = CameraClearFlags.Nothing;
                tempCamera.backgroundColor = Color.clear;
                tempCamera.orthographic = true;
                tempCamera.orthographicSize = 1;
                //tempCamera.cullingMask = 0;


                if (addLight && !isMaskOnly) {


                    // LIGHT
                    GameObject tempLight = new GameObject("CIA TempLight");
                    tempLight.transform.position = tempCamera.transform.position;
                    Vector3 lookAtRotation = (prefab.transform.position - tempCamera.transform.position);
                    if (lookAtRotation != Vector3.zero) {
                        tempLight.transform.rotation = Quaternion.LookRotation(lookAtRotation);
                    }

                    Light lightComponent = tempLight.AddComponent<Light>();
                    lightComponent.type = LightType.Directional;
                    lightComponent.color = lightColor;
                    lightComponent.intensity = lightIntensity;
                    lightComponent.shadows = lightShadows;

                    // Ensure the light is properly applied to the prefab
                    tempLight.transform.parent = tempCamera.transform;

                }


                // Create an instance of the prefab and move it to a temporary location
                Vector3 tempPosition = new Vector3(1000, 1000, 1000); // Far away from other objects
                GameObject tempPrefabInstance = Instantiate(prefab, tempPosition, Quaternion.identity);
                tempPrefabInstance.transform.localScale = prefabScale;
                tempPrefabInstance.transform.eulerAngles = prefabRotation;

                // Calculate bounds and position the camera to frame the prefab
                Bounds bounds = IconGenerationHelper.CalculateBounds(tempPrefabInstance);
                tempCamera.transform.position = bounds.center - Vector3.forward * 10;
                if (iconDimensions.x < 0 || iconDimensions.y < 0) {

                    throw new ArgumentException("Icon dimensions must be greater than 0.");
                }
                // Create a render texture
                RenderTexture renderTexture = new RenderTexture((int)iconDimensions.x, (int)iconDimensions.y, 24);
                //renderTexture.Create();


                // Explicitly clear the render texture before rendering
                GL.Clear(true, true, Color.clear);  // Clear color and depth buffer to ensure no artifacts
                tempCamera.backgroundColor = Color.clear;
                tempCamera.targetTexture = renderTexture;
                tempCamera.Render();

                // Convert the render texture to a texture2D
                RenderTexture.active = renderTexture;
                Texture2D iconTexture = new Texture2D((int)iconDimensions.x, (int)iconDimensions.y, TextureFormat.ARGB32, false);
                iconTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                iconTexture.Apply();

                
                // Add background
                //Texture2D backgroundTexture = iconTexture;
                if (addBackground) {
                    if (!addBorder) {
                        iconTexture = IconGenerationHelper.AddBackgroundFill(iconTexture, radius, 0, borderOffset, backgroundColor, isMaskOnly);
                    } else {
                        iconTexture = IconGenerationHelper.AddBackgroundFill(iconTexture, radius, borderWidth, borderOffset, backgroundColor, isMaskOnly);
                    }
                }

                // Add rounded border
                if (addBorder) {
                    if (ornamentalCorners) {

                        iconTexture = IconGenerationHelper.AddRoundedBorder_Inward(iconTexture, radius, borderWidth, borderOffset, borderColor);

                    } else {

                        iconTexture = IconGenerationHelper.AddRoundedBorder_Crisp(iconTexture, radius, borderWidth, borderOffset, borderColor);
                    }
                }

                // Optionally make the texture a mask
                if (isMaskOnly) {
                    iconTexture = IconGenerationHelper.ConvertToMask(iconTexture, maskColor);
                }

                // Clean up
                DestroyImmediate(tempPrefabInstance);

                // Ensure the camera and render texture references are cleared before destroying
                RenderTexture.active = null;
                tempCamera.targetTexture = null;
                DestroyImmediate(tempCameraObj);
                DestroyImmediate(renderTexture);


                // If generating preview, return the texture without saving
                if (isPreview) return iconTexture;


                // Save the texture as a PNG

                string filePath = $"{savePath}/{fileName}.png";
                if (usePrefabName) {
                    filePath = $"{savePath}/{prefab.name}.png";
                }
                if (!savePath.StartsWith("Assets/")) {
                    savePath = "Assets/" + savePath.TrimStart('/');
                }
                System.IO.File.WriteAllBytes(filePath, iconTexture.EncodeToPNG());
                Debug.Log($"Icon saved to {filePath}");

                // Ensure the texture is imported as Sprite (2D and UI) with Full Rect and Single mode
                AssetDatabase.ImportAsset(filePath, ImportAssetOptions.ForceUpdate);
                TextureImporter importer = AssetImporter.GetAtPath(filePath) as TextureImporter;
                if (importer != null) {

                    // Force reset all settings to ensure consistency
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Single;

                    TextureImporterSettings settings = new TextureImporterSettings();
                    importer.ReadTextureSettings(settings);

                    settings.spriteMode = (int)SpriteImportMode.Single;
                    settings.spriteMeshType = SpriteMeshType.FullRect;
                    settings.textureType = TextureImporterType.Sprite;
                    settings.alphaIsTransparency = true;

                    // Ensure npotScale is set correctly
                    importer.npotScale = TextureImporterNPOTScale.None;


                    importer.SetTextureSettings(settings);

                    // Additional settings to ensure sprite consistency
                    importer.spritePivot = new Vector2(0.5f, 0.5f);
                    importer.spriteBorder = Vector4.zero;

                    importer.SaveAndReimport();
                }

                return null;
            } catch (System.Exception ex) {
                Debug.LogError($"Error during icon generation: {ex.Message}\n{ex.StackTrace}");
                return null;
            }
        }




        private bool VerifySavePath() {

            savePath = savePathField.value;
            // Ensure the folder exists before saving the icon
            //string absoluteSavePath = Path.Combine(Application.dataPath, savePath.Replace("Assets", "").TrimStart(Path.DirectorySeparatorChar));

            // Normalize savePath to ensure it starts with "Assets/"
            if (!savePath.StartsWith("Assets/")) {
                savePath = "Assets/" + savePath.TrimStart('/');
            }

            // Extract the relative path
            string relativePath = savePath.Substring("Assets/".Length);

            // Construct the absolute path
            string absoluteSavePath = Path.Combine(Application.dataPath, relativePath);

            HandleDebug($"Normalized Save Path: {savePath}");
            HandleDebug($"Relative Path: {relativePath}");
            HandleDebug($"Absolute Save Path: {absoluteSavePath}");

            // Ensure the save path exists
            if (EnsureSavePathExists(absoluteSavePath)) {

                return true;
            } else {
                return false;
            }

        }


        private bool EnsureSavePathExists(string path) {
            try {
                // Check if path is null or empty
                if (string.IsNullOrWhiteSpace(path)) {
                    Debug.LogError("Save path is null or empty!");
                    return false;
                }

                // Log full path details
                HandleDebug($"Attempting to create directory at: {path}");
                HandleDebug($"Path exists check: {System.IO.Directory.Exists(path)}");
                HandleDebug($"Full path is absolute: {System.IO.Path.IsPathRooted(path)}");

                // Attempt to create directory with more comprehensive error handling
                if (!System.IO.Directory.Exists(path)) {
                    try {
                        System.IO.Directory.CreateDirectory(path);

                        HandleDebug($"Directory successfully created at: {path}");
                    } catch (System.UnauthorizedAccessException uaEx) {
                        Debug.LogError($"Unauthorized access when creating directory: {uaEx.Message}");
                    } catch (System.IO.IOException ioEx) {
                        Debug.LogError($"IO Exception when creating directory: {ioEx.Message}");
                    } catch (System.Exception Ex) {
                        Debug.LogError($"Unexpected error creating directory: {Ex.Message}");
                    }
                } else {
                    return true;
                }

                // Double-check directory existence after creation attempt
                if (!System.IO.Directory.Exists(path)) {
                    Debug.LogError($"Failed to create directory at: {path}");
                    return false;
                } else {
                    HandleDebug($"Confirmed directory exists at: {path}");
                    return true;
                }
            } catch (System.Exception ex) {
                HandleDebug($"Critical error in EnsureSavePathExists: {ex.Message}");
                return false;
            }
        }


        private void HandleCozyWeatherSphere(bool enable) {

            if (!compatibleWithCozyWeather) return;

            if (cozyWeatherSphere == null) {
                cozyWeatherSphere = GameObject.Find("Cozy Weather Sphere");
                if (cozyWeatherSphere == null) {
                    Debug.LogWarning("Cozy Weather sphere not found in the scene.");
                    return;
                }
            }

            cozyWeatherSphere.SetActive(enable);
            HandleDebug("Cozy Weather is active: " + enable);
        }

        private void HandleDebug(string message, bool isWarning = false) {

            if (debugMode) {
                if (!isWarning) {
                    Debug.Log(message);
                } else {
                    Debug.LogWarning(message);
                }
            }

        }

    }    // END OF CLASS



}   // END OF NAMESPACE