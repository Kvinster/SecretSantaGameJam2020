using UnityEngine;

using System.Collections.Generic;

using SmtProject.Utils;

namespace SmtProject.Core.Platformer {
	public sealed class PlayerController : LazySingleton<PlayerController> {
		const int StartHp    = 100;
		const int StartXp    = 0;
		const int StartLevel = 0;

		static readonly List<int> LevelXps = new List<int> { 1, 3, 6, 10, 15, 22, 30, 40, 60 };

		public readonly ReactValue<int> CurHp = new ReactValue<int>(StartHp);
		public readonly ReactValue<int> CurXp = new ReactValue<int>(StartXp);

		public readonly ReactValue<int> MaxHp       = new ReactValue<int>(StartHp);
		public readonly ReactValue<int> NextLevelXp = new ReactValue<int>(LevelXps[0]);

		public readonly ReactValue<int> CurLevel = new ReactValue<int>(StartLevel);

		public void Reset() {
			CurHp.SetValue(StartHp);
			CurXp.SetValue(StartXp);
			MaxHp.SetValue(StartHp);
			NextLevelXp.SetValue(LevelXps[0]);
			CurLevel.SetValue(StartLevel);
		}

		public void TakeDamage(int damage) {
			CurHp.SetValue(Mathf.Max(CurHp - damage, 0));
		}

		public void RestoreHp() {
			CurHp.SetValue(StartHp);
		}

		public void AddXp(int xp) {
			using ( new DelayedReact() ) {
				CurXp.SetValue(CurXp + xp);
				while ( NextLevelXp.CurValue <= CurXp ) {
					CurLevel.SetValue(CurLevel + 1);
					if ( CurLevel < LevelXps.Count ) {
						NextLevelXp.SetValue(LevelXps[CurLevel]);
					} else {
						break;
					}
				}
			}
		}

		public int GetNextLevelXp(int level) {
			if ( (level >= 0) && (level < LevelXps.Count) ) {
				return LevelXps[level];
			}
			return 0;
		}
	}
}
