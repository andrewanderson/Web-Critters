using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cas.Core
{
    public class GenerationEventArgs : EventArgs
    {
        public int Generation { get; private set; }

        public GenerationEventArgs(int generation)
        {
            this.Generation = generation;
        }
    }
}
