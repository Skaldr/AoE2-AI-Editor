using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoE2_AI_Editor
{
    internal interface AIInstruction
    {
        List<String> getContent();
    }
}
