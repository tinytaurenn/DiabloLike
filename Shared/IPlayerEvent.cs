using System;
using System.Collections.Generic;
using UnityEngine;
using static Shared.S_CharacterSprites;

namespace Shared
{
    public interface IPlayerEvent // Maybe add this directly in IPlayer ?
    {

        public static IPlayerEvent Instance;

        public abstract Action<ESkillSprite, float> OnSkillCast
        {
            get;
        }

        public abstract Action<int> OnLifeChange
        {
            get;
        }

        public abstract Action<int> OnManaChange
        {
            get;
        }
    }
}