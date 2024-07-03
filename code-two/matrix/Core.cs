using SweetPolynomial;
using System.Collections;

namespace MatrixContainer
{
    public static class Compute
    {
        static string MatrixRaw { get; set; }
        static ArrayList Matrices { get; set; }
        static ArrayList MatriceResults { get; set; }
        static Matrix MatrixResult { get; set; }

        public static void SpinComputation(int numberOfLoops)
        {
            MatrixRaw = "1,1;1,2";
            Matrices = new ArrayList();
            MatriceResults = new ArrayList();
            
            for (int i = 0; i < numberOfLoops; i++)
            {
                LoopMatrixMultiply();
            }
        }

        public static void LoopMatrixMultiply()
        {
           var split = MatrixRaw.Split(':');

            for (int i = 0; i < split.Length; i++)
            {
                Matrix M = new Matrix(split[i]);
                Matrices.Add(M);
                Complex det = M.Determinant(); // det = 1 
                Matrix Minv = M.Inverse(); // Minv = [2, -1; -1, 1] 
                
            }
            foreach (Matrix matrix in Matrices)
            {
                Matrix matrixCopy = matrix;
                MatrixResult = matrixCopy * matrix;
                MatriceResults.Add(MatrixResult);
            }
        }
    }
}