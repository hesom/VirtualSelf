using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VirtualSelf
{
    [CreateAssetMenu]
    public class LevelCodes : ScriptableObject
    {
        [System.Serializable]
        public struct LevelMapping
        {
            public string code;
            public string level;
        }

        [SerializeField]
        private List<LevelMapping> levels;

		public string GetLevelFromCode(string code)
		{
			return levels.Find((mapping) => {return mapping.code == code;}).level;
		}

		public string GetCodeFromLevel(string level)
		{
			return levels.Find((mapping) => {return mapping.level == level;}).code;
		}

		public List<LevelMapping> GetAllMappings()
		{
			return levels;
		}
    }
}

