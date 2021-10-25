using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;
using RimWorld;
using Verse;
using UnityEngine;

namespace SyrTraitValue
{
    public class TraitValueExtension : DefModExtension
    {
        public List<DegreeValue> traitValues;
    }

    public class DegreeValue
    {
        public DegreeValue()
        {
        }

        public DegreeValue(int degree, int value)
        {
            this.degree = degree;
            this.value = value;
        }
        public int degree;
        public int value;

        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            if (xmlRoot.ChildNodes.Count != 1) Log.Error("");
            else
            {
                string[] array = xmlRoot.FirstChild.Value.Split(new char[] { ',' });
                if (array.Length == 1)
                {
                    degree = 0;
                    value = ParseHelper.FromString<int>(array[0]);
                }
                else if (array.Length != 2)
                {
                    Log.ErrorOnce(string.Format("Trait values need two numbers seperated by a comma. Or a single number. Wrong value: {0}", xmlRoot.FirstChild.Value), 16205552);
                }
                else
                {
                    degree = ParseHelper.FromString<int>(array[0]);
                    value = ParseHelper.FromString<int>(array[1]);
                }
            }
            //Log.Message("Degree: '" + degree + "' | Value: '" + value + "'");
        }
    }
}
