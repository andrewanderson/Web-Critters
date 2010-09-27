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

        /// <summary>
        /// Generates a random cell
        /// </summary>
        public static ICell New(int tagSize)
        {
            var cell = new GridCell();

            cell.Offense = Tag.New(tagSize, true);
            cell.Defense = Tag.New(tagSize, true);
            cell.Exchange = Tag.New(tagSize, true);

            return cell;
        }

        public static ICell New()
        {
            return new GridCell();
        }

        #endregion
    }
}
