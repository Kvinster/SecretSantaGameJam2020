using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Object = UnityEngine.Object;

namespace SmtProject.Editor {
	public sealed class AnimationGenerationWindow : EditorWindow {
		sealed class ReferencedSpritesheet {
			public List<Sprite>  Sprites;
			public AnimationClip ReferenceClip;

			public bool IsValid => HaveSprites && ReferenceClip;

			bool HaveSprites => (Sprites != null) && (Sprites.Count > 0);

			public void Clear() {
				Sprites?.Clear();
				ReferenceClip = null;
			}
		}

		const string BaseAnimationPath = "Assets/Animations/Platformer/Generated";
		const string BasePrefabPath    = "Assets/Prefabs/Platformer/Generated";

		static readonly string[] Directions = { "Up", "Left", "Down", "Right" };

		DefaultAsset _targetFolder;
		DefaultAsset _referenceFolder;
		bool         _addEvents;

		readonly ReferencedSpritesheet _walkSheet   = new ReferencedSpritesheet();
		readonly ReferencedSpritesheet _hurtSheet   = new ReferencedSpritesheet();
		readonly ReferencedSpritesheet _spellSheet  = new ReferencedSpritesheet();
		readonly ReferencedSpritesheet _bowSheet    = new ReferencedSpritesheet();
		readonly ReferencedSpritesheet _slashSheet  = new ReferencedSpritesheet();
		readonly ReferencedSpritesheet _thrustSheet = new ReferencedSpritesheet();

		AnimationClip _idleReferenceClip;

		string _animName = string.Empty;

		bool HaveName => !string.IsNullOrEmpty(_animName);

		bool ReadyToGenerate => AllValid(_walkSheet, _hurtSheet, _bowSheet, _spellSheet, _slashSheet, _thrustSheet) &&
		                        HaveName;

		void OnGUI() {
			_animName = EditorGUILayout.TextField("Name", _animName);

			var spritesTargetFolder =
				EditorGUILayout.ObjectField("Sprites", _targetFolder, typeof(DefaultAsset), false) as DefaultAsset;
			if ( spritesTargetFolder == null ) {
				_targetFolder = null;
			} else if ( spritesTargetFolder != _targetFolder ) {
				_targetFolder = spritesTargetFolder;
				TryExtractSprites(_targetFolder);
			}

			var referenceFolder =
				EditorGUILayout.ObjectField("References", _referenceFolder, typeof(DefaultAsset), false) as DefaultAsset;
			if ( referenceFolder == null ) {
				_referenceFolder = null;
			} else if ( referenceFolder != _referenceFolder ) {
				_referenceFolder = referenceFolder;
				TryExtractReferenceClips(_referenceFolder);
			}

			_addEvents = EditorGUILayout.Toggle("Add events", _addEvents);

			EditorGUILayout.Space();
			DrawSpritesheetValid("Bow", _bowSheet);
			DrawSpritesheetValid("Hurt", _hurtSheet);
			DrawSpritesheetValid("Spell", _spellSheet);
			DrawSpritesheetValid("Slash", _slashSheet);
			DrawSpritesheetValid("Thrust", _thrustSheet);
			DrawSpritesheetValid("Walk", _walkSheet);
			DrawLabelValid(_idleReferenceClip, $"Idle is {(_idleReferenceClip ? "value" : "invalid")}");
			EditorGUILayout.Space();

			if ( ReadyToGenerate ) {
				if ( GUILayout.Button("Generate") ) {
					GenerateAnimations();
				}
			} else {
				if ( GUILayout.Button("Update values") ) {
					if ( _targetFolder ) {
						TryExtractSprites(_targetFolder);
					}
					if ( _referenceFolder ) {
						TryExtractReferenceClips(_referenceFolder);
					}
				}
			}

			if ( GUILayout.Button("Reset") ) {
				_walkSheet.Clear();
				_hurtSheet.Clear();
				_spellSheet.Clear();
				_bowSheet.Clear();
				_slashSheet.Clear();
				_thrustSheet.Clear();
				_animName = string.Empty;
			}
		}

		void TryExtractSprites(Object folderAsset) {
			var di = new DirectoryInfo(AssetDatabase.GetAssetPath(folderAsset));
			if ( di.Exists ) {
				foreach ( var file in di.GetFiles() ) {
					var path = file.ToString();
					if ( Path.GetExtension(path) != ".png" ) {
						continue;
					}
					path = Path.Combine(di.ToString(), Path.GetFileName(path));
					var asset = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
					if ( asset != null ) {
						var fileName = Path.GetFileNameWithoutExtension(path);
						var index    = fileName.LastIndexOf("_", StringComparison.Ordinal);
						if ( index == -1 ) {
							Debug.LogErrorFormat("Invalid file name '{0}'", fileName);
							return;
						}
						var suffix = fileName.Substring(index + 1, fileName.Length - index - 1);
						switch ( suffix ) {
							case "walk": {
								TryUpdateSpritesheet(asset, _walkSheet);
								break;
							}
							case "bow": {
								TryUpdateSpritesheet(asset, _bowSheet);
								break;
							}
							case "spell": {
								TryUpdateSpritesheet(asset, _spellSheet);
								break;
							}
							case "slash": {
								TryUpdateSpritesheet(asset, _slashSheet);
								break;
							}
							case "thrust": {
								TryUpdateSpritesheet(asset, _thrustSheet);
								break;
							}
							case "hurt": {
								TryUpdateSpritesheet(asset, _hurtSheet);
								break;
							}
						}
					}
				}
			}
		}

		void TryUpdateSpritesheet(Object obj, ReferencedSpritesheet spritesheet) {
			if ( obj && (obj is Texture2D) ) {
				var sprites = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(obj)).Select(x => {
					if ( x && (x is Sprite sprite) ) {
						return sprite;
					}
					return null;
				}).Where(x => (x != null)).ToList();
				var zeroName = sprites[0].name;
				var prefix   = zeroName.Substring(0, zeroName.LastIndexOf('_') + 1);
				sprites.Sort((a, b) => {
					var aIndex = int.Parse(a.name.Replace(prefix, ""));
					var bIndex = int.Parse(b.name.Replace(prefix, ""));
					return aIndex.CompareTo(bIndex);
				});
				if ( sprites.Count > 0 ) {
					spritesheet.Sprites = sprites;
				}
			}
		}

		void TryExtractReferenceClips(Object folderAsset) {
			var di = new DirectoryInfo(AssetDatabase.GetAssetPath(folderAsset));
			if ( di.Exists ) {
				foreach ( var file in di.GetFiles() ) {
					var path = file.ToString();
					if ( Path.GetExtension(path) != ".anim" ) {
						continue;
					}
					path = Path.Combine(di.ToString(), Path.GetFileName(path));
					var asset = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
					if ( asset != null ) {
						var animName = Path.GetFileNameWithoutExtension(path);
						switch ( animName.ToLower() ) {
							case "walk": {
								TryUpdateReferenceClip(asset, _walkSheet);
								break;
							}
							case "bow": {
								TryUpdateReferenceClip(asset, _bowSheet);
								break;
							}
							case "spell": {
								TryUpdateReferenceClip(asset, _spellSheet);
								break;
							}
							case "slash": {
								TryUpdateReferenceClip(asset, _slashSheet);
								break;
							}
							case "thrust": {
								TryUpdateReferenceClip(asset, _thrustSheet);
								break;
							}
							case "hurt": {
								TryUpdateReferenceClip(asset, _hurtSheet);
								break;
							}
							case "idle": {
								_idleReferenceClip = asset;
								break;
							}
						}
					}
				}
			}
		}

		void TryUpdateReferenceClip(Object obj, ReferencedSpritesheet spritesheet) {
			if ( obj && (obj is AnimationClip animationClip) ) {
				spritesheet.ReferenceClip = animationClip;
			}
		}

		void GenerateAnimations() {
			var animationParent = Path.Combine(BaseAnimationPath, _animName);
			var animationDi     = new DirectoryInfo(animationParent);
			if ( !animationDi.Exists ) {
				animationDi.Create();
			}

			var prefabParent = Path.Combine(BasePrefabPath, _animName);
			var prefabDi     = new DirectoryInfo(prefabParent);
			if ( !prefabDi.Exists ) {
				prefabDi.Create();
			}

			var animationPath      = Path.Combine(animationParent, "Controller.asset");
			var animatorController = AnimatorController.CreateAnimatorControllerAtPath(animationPath);

			var prefabPath   = Path.Combine(prefabParent, _animName + ".prefab");
			var instanceRoot = new GameObject(_animName);
			instanceRoot.AddComponent<SpriteRenderer>();
			var animator = instanceRoot.AddComponent<Animator>();
			animator.runtimeAnimatorController = animatorController;
			var prefab = PrefabUtility.SaveAsPrefabAsset(instanceRoot, prefabPath, out var success);
			if ( !success ) {
				Debug.LogError("Can't save instance as prefab");
				return;
			}
			DestroyImmediate(instanceRoot);

			animatorController.AddParameter("IsAlive", AnimatorControllerParameterType.Bool);
			animatorController.AddParameter("IsWalking", AnimatorControllerParameterType.Bool);
			animatorController.AddParameter("IsHitting", AnimatorControllerParameterType.Bool);
			animatorController.AddParameter("WalkDir", AnimatorControllerParameterType.Int);
			animatorController.AddParameter("Slash", AnimatorControllerParameterType.Trigger);
			animatorController.AddParameter("Thrust", AnimatorControllerParameterType.Trigger);
			animatorController.AddParameter("Bow", AnimatorControllerParameterType.Trigger);
			animatorController.AddParameter("Spell", AnimatorControllerParameterType.Trigger);

			GenerateDirectionalAnim("Idle", 36, (i, q) => (i * 9 + q),
				(i) => new [] {
					new AnimatorCondition {
						mode      = AnimatorConditionMode.If,
						parameter = "IsAlive"
					},
					new AnimatorCondition {
						mode      = AnimatorConditionMode.IfNot,
						parameter = "IsWalking"
					},
					new AnimatorCondition {
						mode      = AnimatorConditionMode.IfNot,
						parameter = "IsHitting"
					},
					new AnimatorCondition {
						mode      = AnimatorConditionMode.Equals,
						parameter = "WalkDir",
						threshold = i
					}
				}, animatorController, _walkSheet, prefab, loop: true, _idleReferenceClip);
			GenerateDirectionalAnim("Walk", 36, (i, q) => (i * 9 + (q < 9 ? q : 0)),
				(i) => new [] {
					new AnimatorCondition {
						mode      = AnimatorConditionMode.If,
						parameter = "IsAlive"
					},
					new AnimatorCondition {
						mode      = AnimatorConditionMode.If,
						parameter = "IsWalking"
					},
					new AnimatorCondition {
						mode      = AnimatorConditionMode.IfNot,
						parameter = "IsHitting"
					},
					new AnimatorCondition {
						mode      = AnimatorConditionMode.Equals,
						parameter = "WalkDir",
						threshold = i
					}
				}, animatorController, _walkSheet, prefab, loop: true);
			GenerateDirectionalAnim("Bow", 52, (i, q) => (i * 13 + ((q < 13) ? q : 2 - (q - 13))),
				(i) => new [] {
					new AnimatorCondition {
						mode      = AnimatorConditionMode.If,
						parameter = "IsAlive"
					},
					new AnimatorCondition {
						mode      = AnimatorConditionMode.Equals,
						parameter = "WalkDir",
						threshold = i
					},
					new AnimatorCondition {
						mode      = AnimatorConditionMode.If,
						parameter = "IsHitting"
					},
					new AnimatorCondition {
						mode      = AnimatorConditionMode.If,
						parameter = "Bow",
						threshold = i
					}
				}, animatorController, _bowSheet, prefab);
			GenerateDirectionalAnim("Spellcast", 28, (i, q) => {
				var res = i * 7;
				if ( q >= 6 ) {
					switch ( q ) {
						case 6: {
							q = 4;
							break;
						}
						case 7: {
							q = 6;
							break;
						}
						case 8: {
							q = 0;
							break;
						}
					}
				}
				res += q;
				return res;
			}, (i) => new [] {
				new AnimatorCondition {
					mode      = AnimatorConditionMode.If,
					parameter = "IsAlive"
				},
				new AnimatorCondition {
					mode      = AnimatorConditionMode.Equals,
					parameter = "WalkDir",
					threshold = i
				},
				new AnimatorCondition {
					mode      = AnimatorConditionMode.If,
					parameter = "IsHitting"
				},
				new AnimatorCondition {
					mode      = AnimatorConditionMode.If,
					parameter = "Spell",
					threshold = i
				}
			}, animatorController, _spellSheet, prefab);
			GenerateDirectionalAnim("Slash", 24, (i, q) => (i * 6 + ((q < 6) ? q : 0)),
				(i) => new [] {
					new AnimatorCondition {
						mode      = AnimatorConditionMode.If,
						parameter = "IsAlive"
					},
					new AnimatorCondition {
						mode      = AnimatorConditionMode.Equals,
						parameter = "WalkDir",
						threshold = i
					},
					new AnimatorCondition {
						mode      = AnimatorConditionMode.If,
						parameter = "IsHitting"
					},
					new AnimatorCondition {
						mode      = AnimatorConditionMode.If,
						parameter = "Slash",
						threshold = i
					}
				}, animatorController, _slashSheet, prefab);
			GenerateDirectionalAnim("Thrust", 32, (i, q) => (i * 8 + ((q < 8) ? q : (3 - (q - 8)))),
				(i) => new [] {
					new AnimatorCondition {
						mode      = AnimatorConditionMode.If,
						parameter = "IsAlive"
					},
					new AnimatorCondition {
						mode      = AnimatorConditionMode.Equals,
						parameter = "WalkDir",
						threshold = i
					},
					new AnimatorCondition {
						mode      = AnimatorConditionMode.If,
						parameter = "IsHitting"
					},
					new AnimatorCondition {
						mode      = AnimatorConditionMode.If,
						parameter = "Thrust",
						threshold = i
					}
				}, animatorController, _thrustSheet, prefab);
			GenerateSimpleAnim("Hurt", 6, (i) => (i),
				() => new [] {
					new AnimatorCondition {
						mode      = AnimatorConditionMode.IfNot,
						parameter = "IsAlive"
					},
				}, animatorController, _hurtSheet, prefab);

			EditorUtility.SetDirty(animatorController);
		}

		void GenerateDirectionalAnim(string animName, int spritesheetSize, Func<int, int, int> spriteChooser,
			Func<int, AnimatorCondition[]> conditionFactory, AnimatorController animatorController,
			ReferencedSpritesheet spritesheet, GameObject targetGameObject, bool loop = false,
			AnimationClip overrideReferenceClip = null) {
			var sprites = spritesheet.Sprites;
			if ( sprites.Count != spritesheetSize ) {
				Debug.LogErrorFormat("Invalid spritesheet size for anim '{0}': '{1}'", animName, spritesheetSize);
				return;
			}

			var referenceClip = overrideReferenceClip ? overrideReferenceClip : spritesheet.ReferenceClip;

			var stateMachine = animatorController.layers[0].stateMachine;

			for ( var i = 0; i < Directions.Length; i++ ) {
				var direction = Directions[i];
				var clipName  = $"{animName}{direction}";
				var clip = new AnimationClip {
					name     = clipName,
					wrapMode = loop ? WrapMode.Default : WrapMode.Loop
				};
				if ( loop ) {
					var settings = AnimationUtility.GetAnimationClipSettings(clip);
					settings.loopTime = true;
					AnimationUtility.SetAnimationClipSettings(clip, settings);
				}

				var bindings = AnimationUtility.GetObjectReferenceCurveBindings(referenceClip);
				Debug.Assert(bindings.Length == 1);
				foreach ( var referenceBinding in bindings ) {
					var referenceCurve = AnimationUtility.GetObjectReferenceCurve(referenceClip, referenceBinding);

					var mod = new PropertyModification {
						propertyPath = referenceBinding.path,
					};
					AnimationUtility.PropertyModificationToEditorCurveBinding(mod, targetGameObject, out var binding);
					binding.propertyName = referenceBinding.propertyName;
					binding.path         = referenceBinding.path;
					binding.type         = referenceBinding.type;

					var curve = new ObjectReferenceKeyframe[referenceCurve.Length];
					for ( var q = 0; q < referenceCurve.Length; ++q ) {
						curve[q] = new ObjectReferenceKeyframe {
							time  = referenceCurve[q].time,
							value = sprites[spriteChooser(i, q)]
						};
					}
					AnimationUtility.SetObjectReferenceCurve(clip, binding, curve);
				}

				foreach ( var referenceBinding in AnimationUtility.GetCurveBindings(referenceClip) ) {
					var referenceCurve = AnimationUtility.GetEditorCurve(referenceClip, referenceBinding);
					var mod = new PropertyModification {
						propertyPath = referenceBinding.path,
					};
					AnimationUtility.PropertyModificationToEditorCurveBinding(mod, targetGameObject, out var binding);
					var curve = new AnimationCurve();
					for ( var j = 0; j < referenceCurve.length; ++j ) {
						var referenceKey = referenceCurve.keys[j];
						curve.AddKey(new Keyframe(referenceKey.time, referenceKey.value));
					}
					AnimationUtility.SetEditorCurve(clip, binding, curve);
				}

				if ( _addEvents ) {
					var clipEvents = new List<AnimationEvent>();
					foreach ( var referenceEvent in AnimationUtility.GetAnimationEvents(referenceClip) ) {
						var ev = new AnimationEvent {
							time                     = referenceEvent.time,
							functionName             = referenceEvent.functionName,
							floatParameter           = referenceEvent.floatParameter,
							intParameter             = referenceEvent.intParameter,
							messageOptions           = referenceEvent.messageOptions,
							stringParameter          = referenceEvent.stringParameter,
							objectReferenceParameter = referenceEvent.objectReferenceParameter
						};
						clipEvents.Add(ev);
					}
					if ( clipEvents.Count > 0 ) {
						AnimationUtility.SetAnimationEvents(clip, clipEvents.ToArray());
					}
				}

				AssetDatabase.CreateAsset(clip, AssetDatabase.GetAssetPath(animatorController).Replace("Controller.asset", clipName + ".anim"));
				var animatorState = animatorController.AddMotion(clip);
				animatorState.name = $"{animName}{direction}";

				var transition = stateMachine.AddAnyStateTransition(animatorState);
				transition.duration            = 0f;
				transition.hasExitTime         = false;
				transition.conditions          = conditionFactory(i);
				transition.canTransitionToSelf = false;
			}
		}

		void GenerateSimpleAnim(string animName, int spritesheetSize, Func<int, int> spriteChooser,
			Func<AnimatorCondition[]> conditionFactory, AnimatorController animatorController,
			ReferencedSpritesheet spritesheet, GameObject targetGameObject) {
			var sprites = spritesheet.Sprites;
			if ( sprites.Count != spritesheetSize ) {
				Debug.LogErrorFormat("Invalid spritesheet size for anim '{0}': '{1}'", animName, spritesheetSize);
				return;
			}

			var referenceClip = spritesheet.ReferenceClip;

			var stateMachine = animatorController.layers[0].stateMachine;

			var clip = new AnimationClip {
				name     = animName,
				wrapMode = WrapMode.Loop
			};

			var bindings = AnimationUtility.GetObjectReferenceCurveBindings(referenceClip);
			Debug.Assert(bindings.Length == 1);
			foreach ( var referenceBinding in bindings ) {
				var referenceCurve = AnimationUtility.GetObjectReferenceCurve(referenceClip, referenceBinding);

				var mod = new PropertyModification {
					propertyPath = referenceBinding.path,
				};
				AnimationUtility.PropertyModificationToEditorCurveBinding(mod, targetGameObject, out var binding);
				binding.propertyName = referenceBinding.propertyName;
				binding.path         = referenceBinding.path;
				binding.type         = referenceBinding.type;

				var curve = new ObjectReferenceKeyframe[referenceCurve.Length];
				for ( var i = 0; i < referenceCurve.Length; ++i ) {
					curve[i] = new ObjectReferenceKeyframe {
						time  = referenceCurve[i].time,
						value = sprites[spriteChooser(i)]
					};
				}
				AnimationUtility.SetObjectReferenceCurve(clip, binding, curve);
			}

			foreach ( var referenceBinding in AnimationUtility.GetCurveBindings(referenceClip) ) {
				var referenceCurve = AnimationUtility.GetEditorCurve(referenceClip, referenceBinding);
				var mod = new PropertyModification {
					propertyPath = referenceBinding.path,
				};
				AnimationUtility.PropertyModificationToEditorCurveBinding(mod, targetGameObject, out var binding);
				var curve = new AnimationCurve();
				for ( var j = 0; j < referenceCurve.length; ++j ) {
					var referenceKey = referenceCurve.keys[j];
					curve.AddKey(new Keyframe(referenceKey.time, referenceKey.value));
				}
				AnimationUtility.SetEditorCurve(clip, binding, curve);
			}

			if ( _addEvents ) {
				var clipEvents = new List<AnimationEvent>();
				foreach ( var referenceEvent in AnimationUtility.GetAnimationEvents(referenceClip) ) {
					var ev = new AnimationEvent {
						time                     = referenceEvent.time,
						functionName             = referenceEvent.functionName,
						floatParameter           = referenceEvent.floatParameter,
						intParameter             = referenceEvent.intParameter,
						messageOptions           = referenceEvent.messageOptions,
						stringParameter          = referenceEvent.stringParameter,
						objectReferenceParameter = referenceEvent.objectReferenceParameter
					};
					clipEvents.Add(ev);
				}
				if ( clipEvents.Count > 0 ) {
					AnimationUtility.SetAnimationEvents(clip, clipEvents.ToArray());
				}
			}

			AssetDatabase.CreateAsset(clip, AssetDatabase.GetAssetPath(animatorController).Replace("Controller.asset", animName + ".anim"));
			var animatorState = animatorController.AddMotion(clip);
			animatorState.name = $"{animName}";

			var transition = stateMachine.AddAnyStateTransition(animatorState);
			transition.duration    = 0f;
			transition.hasExitTime = false;
			transition.conditions  = conditionFactory();
		}

		void DrawSpritesheetValid(string spritesheetName, ReferencedSpritesheet spritesheet) {
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

		static bool AllValid(params ReferencedSpritesheet[] spritesheets) {
			foreach ( var spritesheet in spritesheets ) {
				if ( !(spritesheet?.IsValid ?? false) ) {
					return false;
				}
			}
			return true;
		}

		[MenuItem("Tools/AnimationGenerationWindow")]
		static void ShowWindow() {
			GetWindow<AnimationGenerationWindow>(true, "Animation Generation", true);
		}
	}
}
