using UnityEditor;
using UnityEngine;

using ReferencedSpritesheet = SmtProject.Editor.AnimationGenerator.ReferencedSpritesheet;

namespace SmtProject.Editor {
	public sealed class AnimationGenerationWindow : EditorWindow {
		readonly AnimationGenerator _generator = new AnimationGenerator();

		bool _allowIncompleteSpritesheets;

		bool ReadyToGenerate => _generator.ReadyToGenerate;

		bool AddEvents => _generator.AddEvents;

		string       AnimName        => _generator.AnimName;
		DefaultAsset TargetFolder    => _generator.TargetFolder;
		DefaultAsset ReferenceFolder => _generator.ReferenceFolder;

		ReferencedSpritesheet WalkSheet   => _generator.WalkSheet;
		ReferencedSpritesheet HurtSheet   => _generator.HurtSheet;
		ReferencedSpritesheet SpellSheet  => _generator.SpellSheet;
		ReferencedSpritesheet BowSheet    => _generator.BowSheet;
		ReferencedSpritesheet SlashSheet  => _generator.SlashSheet;
		ReferencedSpritesheet ThrustSheet => _generator.ThrustSheet;

		AnimationClip IdleReferenceClip => _generator.IdleReferenceClip;

		void OnGUI() {
			_generator.AnimName = EditorGUILayout.TextField("Name", AnimName);

			var spritesTargetFolder =
				EditorGUILayout.ObjectField("Sprites", TargetFolder, typeof(DefaultAsset), false) as DefaultAsset;
			if ( spritesTargetFolder == null ) {
				_generator.TargetFolder = null;
			} else if ( spritesTargetFolder != TargetFolder ) {
				_generator.TargetFolder = spritesTargetFolder;
				_generator.TryExtractSprites(TargetFolder);
				_generator.AnimName = TargetFolder.name;
			}

			var referenceFolder =
				EditorGUILayout.ObjectField("References", ReferenceFolder, typeof(DefaultAsset), false) as DefaultAsset;
			if ( referenceFolder == null ) {
				_generator.ReferenceFolder = null;
			} else if ( referenceFolder != ReferenceFolder ) {
				_generator.ReferenceFolder = referenceFolder;
				_generator.TryExtractReferenceClips(ReferenceFolder);
			}

			_generator.AddEvents = EditorGUILayout.Toggle("Add events", AddEvents);
			_allowIncompleteSpritesheets =
				EditorGUILayout.Toggle("Allow incomplete sheets", _allowIncompleteSpritesheets);

			EditorGUILayout.Space();
			DrawSpritesheetValid("Bow", BowSheet);
			DrawSpritesheetValid("Hurt", HurtSheet);
			DrawSpritesheetValid("Spell", SpellSheet);
			DrawSpritesheetValid("Slash", SlashSheet);
			DrawSpritesheetValid("Thrust", ThrustSheet);
			DrawSpritesheetValid("Walk", WalkSheet);
			DrawLabelValid(IdleReferenceClip, $"Idle is {(IdleReferenceClip ? "value" : "invalid")}");
			EditorGUILayout.Space();

			if ( _allowIncompleteSpritesheets || ReadyToGenerate ) {
				if ( GUILayout.Button("Generate") ) {
					_generator.GenerateAnimations(_allowIncompleteSpritesheets);
				}
			} else {
				if ( GUILayout.Button("Update values") ) {
					if ( TargetFolder ) {
						_generator.TryExtractSprites(TargetFolder);
					}
					if ( ReferenceFolder ) {
						_generator.TryExtractReferenceClips(ReferenceFolder);
					}
				}
			}

			if ( GUILayout.Button("Reset") ) {
				_generator.Reset();
			}
		}

		void DrawSpritesheetValid(string spritesheetName, AnimationGenerator.ReferencedSpritesheet spritesheet) {
			var isValid   = spritesheet.IsValid;
			DrawLabelValid(isValid, $"{spritesheetName} is {(isValid ? "valid" : "invalid")}");
		}

		void DrawLabelValid(bool isValid, string text) {
			var textColor = isValid ? Color.green : Color.red;
			var style = new GUIStyle(GUI.skin.label) {
				active  = new GUIStyleState { textColor = textColor },
				normal  = new GUIStyleState { textColor = textColor },
				focused = new GUIStyleState { textColor = textColor },
			};
			EditorGUILayout.LabelField(text, style);
		}

		[MenuItem("Tools/AnimationGenerationWindow")]
		static void ShowWindow() {
			GetWindow<AnimationGenerationWindow>(true, "Animation Generation", true);
		}
	}
}
