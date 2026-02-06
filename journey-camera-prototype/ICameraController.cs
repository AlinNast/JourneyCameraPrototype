using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JourneyCameraPrototype
{
    interface ICameraController
    {
        abstract void ChangeToCinematic();

        abstract void ChangeToPlayer();
    }
}
