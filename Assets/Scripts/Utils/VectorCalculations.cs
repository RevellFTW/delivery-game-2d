using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Assets.Scripts.Utils
{
    public static class VectorCalculations
    {
        public static bool CheckIfAiscloseToB(Vector2 a, Transform b)
        {
            if (Vector2.Distance(a, new Vector2(b.position.x, b.position.y)) < 9.5f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
