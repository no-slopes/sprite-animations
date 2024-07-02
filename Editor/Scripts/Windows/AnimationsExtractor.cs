using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpriteAnimations.Editor
{
    public class AnimationsExtractorWindow : EditorWindow
    {
        #region Static

        [MenuItem("Tools/Sprite Animations/Animations Extractor")]
        public static AnimationsExtractorWindow OpenEditorWindow()
        {
            var window = GetWindow<AnimationsExtractorWindow>();
            window.titleContent = new GUIContent("Animations Extractor");
            window.minSize = new Vector2(300, 150);
            window.Show();

            return window;
        }

        #endregion

        #region Fields

        private EnumField _orientationField;
        private IntegerField _originalAnimationsizeField;
        private ObjectField _spriteField;

        private VisualElement _formContainer;
        private VisualElement _selectSpriteTipContainer;

        private IntegerField _columnsField;
        private IntegerField _rowsField;
        private IntegerField _fromField;
        private IntegerField _amountField;
        private Button _extractButton;

        #endregion

        #region Behaviour

        private void OnEnable()
        {
            VisualTreeAsset tree = Resources.Load<VisualTreeAsset>("UI Documents/AnimationsExtractionForm");
            TemplateContainer template = tree.Instantiate();
            _orientationField = template.Q<EnumField>("orientation-field");
            _orientationField.RegisterValueChangedCallback(evt => SetOrientation((ExtractionOrientation)evt.newValue));

            _originalAnimationsizeField = template.Q<IntegerField>("original-sheet-size-field");
            _originalAnimationsizeField.SetValueWithoutNotify(1);
            _originalAnimationsizeField.RegisterValueChangedCallback(evt => EnforceMinValue(_originalAnimationsizeField, evt.newValue, 1));

            _spriteField = template.Q<ObjectField>("sprite-field");
            _spriteField.objectType = typeof(Sprite);
            _spriteField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(evt => OnSpriteFieldChanged(evt.newValue as Sprite));

            _formContainer = template.Q<VisualElement>("form");
            _formContainer.style.display = DisplayStyle.None;

            _selectSpriteTipContainer = template.Q<VisualElement>("select-sprite-tip");
            _selectSpriteTipContainer.style.display = DisplayStyle.Flex;

            _columnsField = template.Q<IntegerField>("columns-field");
            _columnsField.SetValueWithoutNotify(1);
            _columnsField.RegisterValueChangedCallback(evt => EnforceMinValue(_columnsField, evt.newValue, 1));

            _rowsField = template.Q<IntegerField>("rows-field");
            _rowsField.SetValueWithoutNotify(1);
            _rowsField.RegisterValueChangedCallback(evt => EnforceMinValue(_rowsField, evt.newValue, 1));

            _fromField = template.Q<IntegerField>("from-field");
            _fromField.tooltip = "The starting position. If orientation is horizontal and the number of animations is 5, "
                + "meaning the source sprite is 5 animations long in height, setting this value to 3 will "
                + "discard the first 2  rows and start extraction from the third row. You will end up with the animations in "
                + "rows 3, 4 and 5.";

            _fromField.SetValueWithoutNotify(1);
            _fromField.RegisterValueChangedCallback(evt => EnforceMinValue(_fromField, evt.newValue, 1));

            _amountField = template.Q<IntegerField>("amount-field");
            _amountField.SetValueWithoutNotify(1);
            _amountField.RegisterValueChangedCallback(evt => EnforceMinValue(_amountField, evt.newValue, 1));

            SetOrientation((ExtractionOrientation)_orientationField.value);

            _extractButton = template.Q<Button>("extract-button");
            _extractButton.clicked += OnExtractionRequested;

            rootVisualElement.Add(template);

            static void EnforceMinValue(IntegerField field, int newValue, int min)
            {
                if (newValue < min)
                {
                    field.SetValueWithoutNotify(min);
                }
            }
        }

        #endregion

        #region Extracting

        private void OnExtractionRequested()
        {
            Sprite originalSprite = _spriteField.value as Sprite;
            if (originalSprite == null) return;

            ExtractionOrientation orientation = (ExtractionOrientation)_orientationField.value;

            int animationSize = _originalAnimationsizeField.value;
            if (animationSize <= 0)
            {
                Logger.LogError("Animation size cannot be zero or negative.");
                return;
            }

            int from = _fromField.value;
            int amount = _amountField.value;
            int numberOfAnimations = _originalAnimationsizeField.value;

            if (from <= 0)
            {
                Logger.LogError($"From value cannot be 0 or less... Duh!");
                return;
            }

            if (amount <= 0)
            {
                Logger.LogError($"Amount value cannot be 0 or less... Duh!");
                return;
            }

            amount = Mathf.Min(amount, numberOfAnimations - from + 1);

            string assetPath = AssetDatabase.GetAssetPath(_spriteField.value);

            if (string.IsNullOrEmpty(assetPath))
            {
                Logger.LogError($"Could not evaluate sprite. Maybe clearing the source sprite field and trying again?");
                return;
            }

            string unityFolderPath = Path.GetDirectoryName(assetPath);
            string projectRootPath = Path.GetFullPath(Path.Combine(Application.dataPath, @"..\"));

            if (orientation == ExtractionOrientation.Horizontal)
            {
                if (!HorizontalExtraction(originalSprite, from, amount, numberOfAnimations, unityFolderPath, projectRootPath))
                {
                    Logger.LogError("Failed to generate files.");
                    return;
                }
            }
            else
            {
                if (!VerticalExtraction(originalSprite, from, amount, numberOfAnimations, unityFolderPath, projectRootPath))
                {
                    Logger.LogError("Failed to generate files.");
                    return;
                }
            }
        }

        #endregion

        #region Horizontal

        private bool HorizontalExtraction(Sprite sourceSprite, int from, int amount, int numberOfAnimations, string unityFolderPath, string projectRootPath)
        {

            // Sets width and heigh based on orientation
            int spriteWidth = Mathf.FloorToInt(sourceSprite.texture.width / _columnsField.value);
            int spriteHeight = Mathf.FloorToInt(sourceSprite.texture.height / numberOfAnimations);

            int firstIndex = from - 1;

            for (int animationIndex = firstIndex; animationIndex < from + amount - 1; animationIndex++)
            {
                // Only recognize sprites with colored pixels to prevent creating empty sprites
                List<Texture2D> recognizedTextures = new();
                for (int i = 0; i < _columnsField.value; i++)
                {
                    Color[] spritePixels = sourceSprite.texture.GetPixels(i * spriteWidth, animationIndex * spriteHeight, spriteWidth, spriteHeight);
                    if (HasPaintedPixels(spritePixels))
                    {
                        Texture2D newTexture = new(spriteWidth, spriteHeight);
                        newTexture.SetPixels(spritePixels);
                        recognizedTextures.Add(newTexture);
                    }
                    else
                    {
                        continue;
                    }
                }

                if (recognizedTextures.Count == 0)
                {
                    Logger.LogWarning($"No textures found for file {animationIndex + 1}.");
                    continue;
                }

                Texture2D result = new(spriteWidth * recognizedTextures.Count, spriteHeight);

                for (int i = 0; i < recognizedTextures.Count; i++)
                {
                    result.SetPixels(i * spriteWidth, 0, spriteWidth, spriteHeight, recognizedTextures[i].GetPixels());
                }

                byte[] bytes = result.EncodeToPNG();
                string filePath = $"{unityFolderPath}/{sourceSprite.texture.name}_{animationIndex + 1}.png";

                string fullPath = Path.Combine(projectRootPath, filePath);
                File.WriteAllBytes(fullPath, bytes);
                ImportHorizontalSpriteAsset(sourceSprite, recognizedTextures.Count, filePath);
            }

            return true;
        }

        private void ImportHorizontalSpriteAsset(Sprite originalSprite, int spriteCount, string path)
        {
            List<SpriteRect> spriteRects = new();
            int ppu = Mathf.FloorToInt(originalSprite.pixelsPerUnit);
            AssetDatabase.ImportAsset(path);
            Sprite importedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
            ti.isReadable = true;
            ti.spriteImportMode = SpriteImportMode.Multiple;
            ti.spritePixelsPerUnit = ppu;

            int size = Mathf.Max(importedSprite.texture.width, importedSprite.texture.height);
            if (size <= 2048)
            {
                ti.maxTextureSize = 2048;
            }
            else if (size <= 4096)
            {
                ti.maxTextureSize = 4096;
            }
            else
            {
                ti.maxTextureSize = 8192;
            }

            ti.filterMode = originalSprite.texture.filterMode;
            ti.textureCompression = TextureImporterCompression.Uncompressed;
            ti.wrapMode = originalSprite.texture.wrapMode;

            var importerSettings = new TextureImporterSettings();
            ti.ReadTextureSettings(importerSettings);
            importerSettings.spriteGenerateFallbackPhysicsShape = false;
            ti.SetTextureSettings(importerSettings);

            var factory = new SpriteDataProviderFactories();
            factory.Init();
            var dataProvider = factory.GetSpriteEditorDataProviderFromObject(ti);
            dataProvider.InitSpriteEditorDataProvider();

            int sliceWidth = Mathf.FloorToInt(importedSprite.texture.width / spriteCount);
            int sliceHeight = importedSprite.texture.height;

            for (int i = 0; i < spriteCount; i++)
            {
                int x = i * sliceWidth;
                int y = 0;
                SpriteRect spriteRect = new()
                {
                    name = $"{importedSprite.name}_{i}",
                    pivot = originalSprite.pivot,
                    rect = new Rect(x, y, sliceWidth, sliceHeight)
                };
                spriteRects.Add(spriteRect);
            }
            dataProvider.SetSpriteRects(spriteRects.ToArray());
            dataProvider.Apply();

            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            AssetDatabase.SaveAssets();
        }

        #endregion

        #region Vertical

        private bool VerticalExtraction(Sprite sourceSprite, int from, int amount, int numberOfAnimations, string unityFolderPath, string projectRootPath)
        {
            // Sets width and heigh based on orientation
            int spriteWidth = Mathf.FloorToInt(sourceSprite.texture.width / numberOfAnimations);
            int spriteHeight = Mathf.FloorToInt(sourceSprite.texture.height / _rowsField.value);

            int firstIndex = from - 1;

            for (int animationIndex = firstIndex; animationIndex < from + amount - 1; animationIndex++)
            {
                // Only recognize sprites with colored pixels to prevent creating empty sprites
                List<Texture2D> recognizedTextures = new();
                for (int i = 0; i < _rowsField.value; i++)
                {
                    Color[] spritePixels = sourceSprite.texture.GetPixels(animationIndex, i * spriteHeight, spriteWidth, spriteHeight);
                    if (HasPaintedPixels(spritePixels))
                    {
                        Texture2D newTexture = new(spriteWidth, spriteHeight);
                        newTexture.SetPixels(spritePixels);
                        recognizedTextures.Add(newTexture);
                    }
                    else
                    {
                        continue;
                    }
                }

                if (recognizedTextures.Count == 0)
                {
                    Logger.LogWarning($"No textures found for file {animationIndex + 1}.");
                    continue;
                }

                Texture2D result = new(spriteWidth, spriteHeight * recognizedTextures.Count);

                for (int i = 0; i < recognizedTextures.Count; i++)
                {
                    result.SetPixels(0, i * spriteHeight, spriteWidth, spriteHeight, recognizedTextures[i].GetPixels());
                }

                byte[] bytes = result.EncodeToPNG();
                string filePath = $"{unityFolderPath}/{sourceSprite.texture.name}_{animationIndex + 1}.png";

                string fullPath = Path.Combine(projectRootPath, filePath);
                File.WriteAllBytes(fullPath, bytes);
                ImportVerticalSpriteAsset(sourceSprite, recognizedTextures.Count, filePath);
            }

            return true;
        }


        private void ImportVerticalSpriteAsset(Sprite originalSprite, int spriteCount, string path)
        {
            List<SpriteRect> spriteRects = new();
            int ppu = Mathf.FloorToInt(originalSprite.pixelsPerUnit);
            AssetDatabase.ImportAsset(path);
            Sprite importedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
            ti.isReadable = true;
            ti.spriteImportMode = SpriteImportMode.Multiple;
            ti.spritePixelsPerUnit = ppu;

            int size = Mathf.Max(importedSprite.texture.width, importedSprite.texture.height);
            if (size <= 2048)
            {
                ti.maxTextureSize = 2048;
            }
            else if (size <= 4096)
            {
                ti.maxTextureSize = 4096;
            }
            else
            {
                ti.maxTextureSize = 8192;
            }

            ti.filterMode = originalSprite.texture.filterMode;
            ti.textureCompression = TextureImporterCompression.Uncompressed;
            ti.wrapMode = originalSprite.texture.wrapMode;

            var importerSettings = new TextureImporterSettings();
            ti.ReadTextureSettings(importerSettings);
            importerSettings.spriteGenerateFallbackPhysicsShape = false;
            ti.SetTextureSettings(importerSettings);

            var factory = new SpriteDataProviderFactories();
            factory.Init();
            var dataProvider = factory.GetSpriteEditorDataProviderFromObject(ti);
            dataProvider.InitSpriteEditorDataProvider();

            int sliceWidth = importedSprite.texture.width;
            int sliceHeight = Mathf.FloorToInt(importedSprite.texture.height / spriteCount);

            for (int i = 0; i < spriteCount; i++)
            {
                int x = 0;
                int y = i * sliceHeight;
                SpriteRect spriteRect = new()
                {
                    name = $"{importedSprite.name}_{i}",
                    pivot = originalSprite.pivot,
                    rect = new Rect(x, y, sliceWidth, sliceHeight)
                };
                spriteRects.Add(spriteRect);
            }
            dataProvider.SetSpriteRects(spriteRects.ToArray());
            dataProvider.Apply();

            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            AssetDatabase.SaveAssets();
        }

        #endregion

        #region UI Fields

        private void OnSpriteFieldChanged(Sprite newSprite)
        {
            _formContainer.style.display = newSprite == null ? DisplayStyle.None : DisplayStyle.Flex;
            _selectSpriteTipContainer.style.display = newSprite == null ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void SetOrientation(ExtractionOrientation orientation)
        {
            _columnsField.style.display = orientation == ExtractionOrientation.Horizontal ? DisplayStyle.Flex : DisplayStyle.None;
            _rowsField.style.display = orientation == ExtractionOrientation.Vertical ? DisplayStyle.Flex : DisplayStyle.None;
        }

        #endregion

        #region Utils

        private bool HasPaintedPixels(Color[] pixels)
        {
            foreach (Color pixel in pixels)
            {
                if (pixel.a > 0)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

    }
}