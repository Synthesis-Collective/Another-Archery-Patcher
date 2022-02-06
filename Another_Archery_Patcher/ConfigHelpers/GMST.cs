using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using System;
using System.Collections.Generic;

namespace Another_Archery_Patcher.ConfigHelpers
{
    public class GMST<T> where T : unmanaged
    {
        public GMST(string[] names, T? value)
        {
            myNames = names;
            Enable = value != null;
            Value = value;
        }
        public GMST(string name, T? value)
        {
            myNames = new string[] { name };
            Enable = value != null;
            Value = value;
        }

        private readonly string[] myNames;
        public bool Enable;
        public T? Value;

        public T GetValue(T current, out bool changed)
        {
            changed = Enable && !Value.Equals(current);
            return changed
                ? (T)Value!
                : current;
        }

        private IEnumerable<string> Names { get { return myNames; } }

        public int AddAllSettings(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            int count = 0;

            if (Enable)
            {                
                foreach (var name in Names)
                {
                    if (typeof(T) == typeof(bool))
                    {
                        state.PatchMod.GameSettings.Add(new GameSettingBool(state.PatchMod.GetNextFormKey(), state.PatchMod.SkyrimRelease)
                        {
                            EditorID = name,
                            Data = Convert.ToBoolean(Value)
                        });
                        ++count;
                    }
                    else if (typeof(T) == typeof(float))
                    {
                        state.PatchMod.GameSettings.Add(new GameSettingFloat(state.PatchMod.GetNextFormKey(), state.PatchMod.SkyrimRelease)
                        {
                            EditorID = name,
                            Data = (float)Convert.ToDecimal(Value)
                        });
                        ++count;
                    }
                    else if (typeof(T) == typeof(int))
                    {
                        state.PatchMod.GameSettings.Add(new GameSettingInt(state.PatchMod.GetNextFormKey(), state.PatchMod.SkyrimRelease)
                        {
                            EditorID = name,
                            Data = Convert.ToInt32(Value)
                        });
                        ++count;
                    }
                    else throw new TypeAccessException($"Unsupported GMST type: {typeof(T)}");
                }
            }

            return count;
        }
    }
}
