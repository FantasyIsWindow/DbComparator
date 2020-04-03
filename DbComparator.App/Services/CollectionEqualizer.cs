using Comparator.Repositories.Models.DtoModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DbComparator.App.Services
{
    public class CollectionEqualizer
    {
        private List<string> _generalTemplate;

        /// <summary>
        /// String collection alignment function
        /// </summary>
        /// <param name="lsCol">Left side collection</param>
        /// <param name="rsCol">Right side collection</param>
        public void GeneralEqualizer(List<string> lsCol, List<string> rsCol)
        {
            List<string> lsTempCol = new List<string>(lsCol);
            List<string> rsTempCol = new List<string>(rsCol);

            CreateGeneralTemplate(lsTempCol, rsTempCol);

            Equalize(lsTempCol, lsCol);
            Equalize(rsTempCol, rsCol);
        }

        /// <summary>
        /// Creating a reference template for comparison
        /// </summary>
        /// <param name="ls">Left side collection</param>
        /// <param name="rs">Right side collection</param>
        private void CreateGeneralTemplate(IEnumerable<string> ls, IEnumerable<string> rs) =>
            _generalTemplate = ls.Union(rs).Distinct().OrderBy(n => n).ToList();

        /// <summary>
        /// Alignment of the collection
        /// </summary>
        /// <param name="tempCol">Temp collection</param>
        /// <param name="writableCol">Writable collection</param>
        private void Equalize(IEnumerable<string> tempCol, List<string> writableCol)
        {
            writableCol.Clear();

            for (int i = 0; i < _generalTemplate.Count; i++)
            {
                if (_generalTemplate[i] == null && tempCol.Contains(_generalTemplate[i]))
                {
                    writableCol.Add("null");
                }
                else if (tempCol.Contains(_generalTemplate[i]))
                {
                    writableCol.Add(_generalTemplate[i]);
                }
                else
                {
                    writableCol.Add("null");
                }
            }
        }

        /// <summary>
        /// Field collection alignment function
        /// </summary>
        /// <param name="lsCol">Left side collection</param>
        /// <param name="rsCol">Right side collection</param>
        public void FieldsAlignment(ObservableCollection<DtoFullField> lsCol, ObservableCollection<DtoFullField> rsCol)
        {
            if (lsCol == null || rsCol == null)
            {
                return;
            }

            if (lsCol.Count > rsCol.Count)
            {
                Alignment(lsCol, rsCol);
            }
            else
            {
                Alignment(rsCol, lsCol);
            }
        }

        /// <summary>
        /// Alignment of the collection
        /// </summary>
        /// <param name="lsCol">Left side collection</param>
        /// <param name="rsCol">Right side collection</param>
        private void Alignment(ObservableCollection<DtoFullField> lsCol, ObservableCollection<DtoFullField> rsCol)
        {
            int size = lsCol.Count - rsCol.Count;

            for (int i = 0; i < size; i++)
            {
                var field = new DtoFullField()
                {
                    ConstraintKeys = "",
                    ConstraintType = "",
                    ConstraintName = "",
                    FieldName      = "",
                    IsNullable     = "",
                    OnDelete       = "",
                    OnUpdate       = "",
                    Referenced     = "",
                    Size           = "",
                    TypeName       = ""
                };
                rsCol.Add(field);
            }
        }
    }
}
