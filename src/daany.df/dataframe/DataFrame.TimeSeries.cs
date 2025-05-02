//////////////////////////////////////////////////////////////////////////////
//   ____    _    _   _   _   __  __                                       //
//  |  _ \  / \  | \ | | | \ | |\ \/ /                                     //
//  | | | |/ _ \ |  \| | |  \| | \  /                                      //
//  | |_| / ___ \| |\  | | |\  | | |                                       //
//  |____/_/   \_\_| \_| |_| \_| |_|                                       //
//                                                                         //
//  DAata ANalYtics Library                                                //
//  Daany.DataFrame:Implementation of DataFrame.                           //
//  https://github.com/bhrnjica/daany                                      //
//                                                                         //
//  Copyright © 20019-2025 Bahrudin Hrnjica                                //
//                                                                         //
//  Free. Open Source. MIT Licensed.                                       //
//  https://github.com/bhrnjica/daany/blob/master/LICENSE                  //
//////////////////////////////////////////////////////////////////////////////

using System.Linq;
using System.Collections.Generic;

using Daany.Interfaces;

namespace Daany
{
	public partial class DataFrame
    {

        public static DataFrame CreateTimeSeries(DataFrame df, int pastSteps, int futureSteps = 1)
        {

            var cols = generateCols(df.Columns, pastSteps, futureSteps);
            var ind = generateIndex(df.Index, pastSteps, futureSteps);

            //data generation
            var values = new List<object?>();
            for(int i=0; i< ind.Count; i++)
            {
                for (int j = 0; j < df.Columns.Count; j++)
                {
                    var col = df.Columns[j];
                   
                    var tsColRow = df[col].Skip(i).Take(pastSteps + futureSteps);
                    values.AddRange(tsColRow);
                }
            }

            //
            return new DataFrame(values, ind, cols);
        }

        private static List<string> generateCols(List<string> columns, int pastSteps, int futureSteps)
        {
            //create columns for new data-frame
            var cols = new List<string>();
            for (int j = 0; j < columns.Count; j++)
            {
                int counter = 1;
                var col = columns[j];
                for (int i = pastSteps + futureSteps; i > 0; i--)
                {
                    if (i <= futureSteps)
                    {
                        if (futureSteps == 1)
                            cols.Add(col);
                        else
                        {
                            cols.Add($"{col}_t{counter}");
                            counter++;
                        }

                    }
                    else
                        cols.Add($"{col}_lag{i - futureSteps}");
                }
            }
            return cols;
        }

        private static List<object> generateIndex(Index ind, int pastSteps, int futureSteps)
        {
            int rows = ind.Count-pastSteps;
            var val = ind.Skip(pastSteps).Take(rows - futureSteps + 1).ToList();
            var newIndex = new Index(val, ind.Name);
            return val;
        }


    }
}
