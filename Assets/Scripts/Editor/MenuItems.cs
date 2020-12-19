using UnityEditor;
using UnityEngine;

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SmtProject.Editor {
	public static class MenuItems {
		static readonly List<Vector2Int> Sizes = new List<Vector2Int> {
			new Vector2Int(13, 4),
			new Vector2Int(6, 1),
			new Vector2Int(6, 4),
			new Vector2Int(7, 4),
			new Vector2Int(8, 4),
			new Vector2Int(9, 4),
		};

		[MenuItem("Tools/Slice Smages")]
		static void Splice() {
			var objs = Selection.objects;
			foreach ( var obj in objs ) {
				if ( !(obj is DefaultAsset defaultAsset) ) {
					Debug.LogError("Unexpected asset type");
					continue;
				}
				var di = new DirectoryInfo(AssetDatabase.GetAssetPath(defaultAsset));
				if ( !di.Exists ) {
					Debug.LogError("Directory doesn't exist");
					continue;
				}
				var fis = di.GetFiles().Where(x => Path.GetExtension(x.Name) == ".png").ToArray();
				if ( fis.Length != Sizes.Count ) {
					Debug.LogError("Unexpected files count");
					continue;
				}
				for ( var i = 0; i < fis.Length; i++ ) {
					var fi       = fis[i];
					var filePath = Path.Combine(di.ToString(), fi.Name);
					var texture  = AssetDatabase.LoadAssetAtPath<Texture2D>(filePath);
					if ( !texture ) {
						Debug.LogError("Can't load sprite");
						continue;
					}
					var textureWidth  = texture.width;
					var textureHeight = texture.height;
					var importer      = AssetImporter.GetAtPath(filePath) as TextureImporter;
					if ( importer == null ) {
						Debug.LogError("Can't get TextureImporter");
						continue;
					}
					importer.spriteImportMode = SpriteImportMode.Single;
					AssetDatabase.Refresh();
					importer.spriteImportMode = SpriteImportMode.Multiple;
					var spritesheet = new List<SpriteMetaData>();
					var size        = Sizes[i];
					var metaSize    = new Vector2Int(textureWidth / size.x, textureHeight / size.y);
					var count       = 0;
					for ( var row = size.y - 1; row >= 0; --row ) {
						for ( var column = 0; column < size.x; ++column ) {
							var metaData = new SpriteMetaData {
								name = $"{texture.name}_{count++}",
								rect = new Rect(column * metaSize.x, row * metaSize.y, metaSize.x, metaSize.y)
							};
							spritesheet.Add(metaData);
						}
					}
					importer.spritesheet = spritesheet.ToArray();
					importer.filterMode = FilterMode.Point;
					importer.SaveAndReimport();
				}
			}
			AssetDatabase.Refresh();
		}

		[MenuItem("Tools/Generate All")]
		static void GenerateAll() {
			var generator = new AnimationGenerator {
				AddEvents       = false,
				ReferenceFolder = AnimationGenerator.DefaultReferenceFolder
			};

			var objs = Selection.objects;
			foreach ( var obj in objs ) {
				if ( !(obj is DefaultAsset defaultAsset) ) {
					Debug.LogError("Unexpected asset type");
					continue;
				}
				generator.TargetFolder = defaultAsset;
				generator.TryExtractSprites(generator.TargetFolder);
				generator.TryExtractReferenceClips(generator.ReferenceFolder);
				generator.AnimName = generator.TargetFolder.name;
				if ( generator.ReadyToGenerate ) {
					generator.GenerateAnimations();
				} else {
					Debug.LogError("Not ready to generate");
				}
				generator.Reset();
			}
		}
	}
}
