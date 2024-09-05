using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoE2_AI_Editor
{
    internal class Comment : AIInstruction
    {

        private int start;
        private int end;
        public Comment(int strat)
        {

        }
        public int getEnd()
        {
            return end;
        }

        public int getStart()
        {
            return start;
        }
    }
}
