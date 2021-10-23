using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDT.Web.Models
{
    public class Matrix
    {
        private List<Requirement> requirements;
        private List<UseCase> useCases;

        public Matrix(List<Requirement> requirements, List<UseCase> useCases)
        {
            this.requirements = requirements;
            this.useCases = useCases;
        }

        public SortedDictionary<Point, Object> GetMatrix()
        {
            SortedDictionary<Point, Object> matrix = new SortedDictionary<Point, Object>();
            int n = 1;
            foreach (var item in requirements)
            {
                Point point = new Point(0, n);
                matrix.Add(point, item);
                n++;
            }
            n = 1;
            foreach (var item in useCases)
            {
                Point point = new Point(n, 0);
                matrix.Add(point, item);
                n++;
            }

            for (int i = 0; i < requirements.Count; i++)
            {
                Requirement requirement = requirements[i];
                for (int j = 0; j < useCases.Count; j++)
                {
                    UseCase useCase = useCases[j];
                    foreach(var item in requirement.UseCaseRequirements)
                    {
                        if (useCase.ID == item.ID_UseCase)
                        {
                            Point point = new Point(j+1, i+1);
                            matrix.Add(point, true);
                        }
 
                    }

                }
            }



            return matrix;
        }
    }
}