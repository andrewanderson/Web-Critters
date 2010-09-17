using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cas.Core;
using Cas.Core.Interfaces;

namespace Cas.TestImplementation
{
    public class GridCell : CellBase
    {
        // TODO: 
        //       For observational purposes we'll need to give each Cell some sort of structure that
        //       records its activities from generation to generation.  i.e. List<IInteraction>

        private GridCell() : base()
        {
            // ???
        }

        public override ICell CreateEmptyCell()
        {
            return new GridCell();
        }
        
        #region Static members

        public const int DefaultTagSize = 4;

        public const int MaxOffenseSize = DefaultTagSize;
        public const int MaxDefenseSize = DefaultTagSize;
        public const int MaxExchangeSize = DefaultTagSize;

        /// <summary>
        /// Generates a random cell
        /// </summary>
        public static ICell New(bool seedRandom)
        {
            var cell = new GridCell();

            if (seedRandom)
            {
                cell.Offense = Tag.New(MaxOffenseSize, true);
                cell.Defense = Tag.New(MaxDefenseSize, true);
                cell.Exchange = Tag.New(MaxExchangeSize, true);
            }

            return cell;
        }

        #endregion
    }
}
