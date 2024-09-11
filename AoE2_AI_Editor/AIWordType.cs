using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoE2_AI_Editor
{
    enum AIWordType
    {
        Defrule,
        Defconst,
        LoadIfDefined,
        RuleCondition,
        Operator,
        Object,
        Action,
        Comment,
        None
    }
}

public static class Constants
{
    const string KEY_DEFRULE = "defrule";
    const string KEY_LOAD_IF_DEFINED = "#load-if-defined";
    const string KEY_LOAD_IF_NOT_DEFINED = "#load-if-not-defined";
    const string KEY_ELSE = "#else";
    const string KEY_END = "#end-if";
    const string KEY_THEN = "=>";

}
