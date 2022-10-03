﻿using KBE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBE.Components
{
    public class KanjiWordComparer : Comparer<KanjiWord>
    {
        public override int Compare(KanjiWord? x, KanjiWord? y)
        {
            if (x is null || y is null)
            {
                return 0;
            }
            return x.Kanji.CompareTo(y.Kanji);
        }
    }
}
