using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LaserPolishingModel.Util.MathNetExtensions;
using static LaserPolishingModel.Util.IEnumerableExtensions;

namespace LaserPolishingModel.Util
{
    public static class Ripples
    {
        const double GRIDSET = 0.2;

        public static Matrix<double> GenerateRipples(Vector<double> xVec, Vector<double> yVec, double afs, double d, double spot, double xStep, double yStep)
        {
            var r_spot = spot / 2;
            var ifs_inner = afs;
            var r_melt = d / 2;
            var r_peak = r_melt - r_spot;

            var ifs_outer = (ifs_inner * Math.Pow(r_peak, 3)) / (Math.Pow(r_peak, 3) - Math.Pow(r_melt, 3));
            var z_valley = -ifs_outer * (r_melt - r_peak);
            var z_peak = z_valley - (ifs_inner * r_peak);

            // generate single spot
            var spot_row_size = (int) Math.Ceiling(2 * r_melt / GRIDSET);
            var spot_col_size = spot_row_size;
            Matrix<double> spotShape = Matrix<double>.Build.Dense(spot_row_size, spot_col_size);
            Matrix<double> circularRegion = Matrix<double>.Build.Dense(spot_row_size, spot_col_size);

            var outer_region_offset = z_peak - ifs_outer * r_peak;
            Parallel.For(0, spot_row_size * spot_col_size, (i) =>
            {
                var row = i / spot_row_size;
                var col = i % spot_row_size;

                var x = (r_melt / xVec.Count()) * col - r_melt;
                var y = (-r_melt / yVec.Count()) * row + r_melt;

                var location = Math.Pow(x, 2) + Math.Pow(y, 2);

                if (location < Math.Pow(r_peak, 2))
                    spotShape[row, col] = ifs_inner * Math.Sqrt(location) + z_valley;
                else
                    spotShape[row, col] = ifs_outer * Math.Sqrt(location) + outer_region_offset;

                if (location < Math.Pow(r_peak + GRIDSET, 2))
                    circularRegion[row, col] = spotShape[row, col];
            });

            // overlap area & # of spots
            var roit_proj_area = Math.PI * Math.Pow(r_melt, 2);
            var x_area_overlap = AreaOverlap(r_melt, xStep);
            var y_area_overlap = AreaOverlap(r_melt, yStep);

            var x_overlap_pct = x_area_overlap / roit_proj_area;
            var y_overlap_pct = y_area_overlap / roit_proj_area;

            var x_spots = (int) (2 * Math.Ceiling(1 / (1 - x_overlap_pct)));
            var y_spots = (int) (2 * Math.Ceiling(1 / (1 - y_overlap_pct)));

            var grid_x_limits = Range(-r_melt, (x_spots - 1) * xStep + r_melt, GRIDSET);
            var grid_y_limits = Range(-r_melt, (y_spots - 1) * yStep + r_melt, GRIDSET);
            //var grid_coordinates = meshgrid(grid_x_limits, grid_y_limits);

            // create large grid
            Matrix<double> overlay_grid = Matrix<double>.Build.Dense(grid_y_limits.Count(), grid_x_limits.Count());

            // lay spots down in a snake-like fashion
            // i.e. (1, 1) -> (1, n) -> (2, n) -> (2, 1) -> (3, 1) ...

            var forward_y_list = Range(0, y_spots, 1);
            var backward_y_list = Range(y_spots - 1, 0, -1);

            for (int i = 0; i < x_spots; i++)
            {
                foreach (var j in (i % 2 == 1) ? forward_y_list : backward_y_list)
                {
                    var x_center_index = (int) (((i * xStep) + r_melt) / GRIDSET); // this gets us the true coordinate. 
                    var y_center_index = (int) (((j * yStep) + r_melt) / GRIDSET);
                    var x_start = x_center_index - (spot_col_size / 2);
                    var x_end = x_center_index + (spot_col_size / 2);
                    var y_start = y_center_index - (spot_row_size / 2);
                    var y_end = y_center_index + (spot_row_size / 2);

                    Parallel.For(x_start, x_end + 1, (x_index) =>
                    {
                        for (int y_index = y_start; y_index <= y_end; y_index++)
                        {
                            var location = Math.Pow(x_index - x_center_index, 2) + Math.Pow(y_index - y_center_index, 2);
                            if (location < Math.Pow((r_peak / GRIDSET) + 1, 2))
                                overlay_grid[y_index, x_index] = spotShape[y_index - y_start, x_index - x_start];
                        }
                    });
                }
            }

            // grab the lattice
            var unit_cell_start_row = (int) (r_melt / GRIDSET);
            var unit_cell_start_col = (int)(r_melt / GRIDSET);
            var unit_cell_num_rows = (int)(((x_spots - 1) * xStep - r_melt) / GRIDSET) - unit_cell_start_row;
            var unit_cell_num_cols = (int)(((y_spots - 1) * yStep - r_melt) / GRIDSET) - unit_cell_start_col;

            var unit_cell = overlay_grid.SubMatrix(unit_cell_start_row, unit_cell_num_rows, unit_cell_start_col, unit_cell_num_cols);
            unit_cell -= overlay_grid.RowSums().Sum() / (overlay_grid.ColumnCount * overlay_grid.RowCount);

            // now replicate the unit cell to a size close to the input dimension
            int num_cols = (int) (2 * Math.Ceiling((xVec.Count + 0.0) / unit_cell.ColumnCount));
            int num_rows = (int) (2 * Math.Ceiling((yVec.Count + 0.0) / unit_cell.RowCount));
            var lattice = unit_cell.repmat(num_rows, num_cols);
            
             



            return overlay_grid;
        }

        static double AreaOverlap(double radius, double step)
        {
            return 2 * Math.Pow(radius, 2) * Math.Acos(step / (2 * radius)) - (step / 2) * Math.Sqrt(4 * Math.Pow(radius, 2) - Math.Pow(step, 2));
        }

        /*
    %% Unit cell to lattice
    
    % now take the unit cell and replicate to a size close to XI * YI
    % once again, 2x factor is for fudge factor to allow interp2 to do a
    % better job
    lattice = repmat(unit_cell, 2 * ceil(size(XI, 2) / size(unit_cell, 2)), 2 *ceil(size(YI, 1) / size(unit_cell, 1)));
    lattice_x = 0:gridset:gridset * (size(lattice, 2) - 1);
    lattice_y = 0:gridset:gridset * (size(lattice, 1) - 1);
    lattice_y = lattice_y';
    
    %% Convert to Experimental Mesh
    lattice_x = lattice_x / 1000;
    lattice_y = lattice_y / 1000;
    
    ZI = interp2(lattice_x, lattice_y, lattice, XI, YI);*/
    }
}
