using System.Collections.Generic;
using System.Linq;

namespace DbComparator.App.Services
{
    public class CollectionEqualizer
    {
        private List<string> _generalTemplate;

        public void CollectionsEquation(List<string> primaryCol, List<string> secondaryCol)
        {
            List<string> tempPrimaryCol = new List<string>(primaryCol);
            List<string> tempSecondaryCol = new List<string>(secondaryCol);

            CreateGeneralTemplate(tempPrimaryCol, tempSecondaryCol);

            Equalize(tempPrimaryCol, primaryCol);
            Equalize(tempSecondaryCol, secondaryCol);
        }

        private void CreateGeneralTemplate(IEnumerable<string> first, IEnumerable<string> second) =>
            _generalTemplate = first.Union(second).Distinct().OrderBy(n => n).ToList();

        private void Equalize(IEnumerable<string> tempCol, List<string> writableCol)
        {
            writableCol.Clear();

            for (int i = 0; i < _generalTemplate.Count; i++)
            {
                if (tempCol.Contains(_generalTemplate[i]))
                {
                    writableCol.Add(_generalTemplate[i] );
                }
                else
                {
                    writableCol.Add("null" );
                }
            }
        } 
    }
}
