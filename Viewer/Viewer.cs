using Core.Data;
using Core.Helper;
using Core.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Viewer
{
    public partial class Viewer : Form
    {
        private Parameter Parameter;
        private LDAModel LDAModel;

        public Viewer()
        {
            InitializeComponent();
        }

        private void buttonLoadModel_Click(object sender, EventArgs e)
        {
            if (modelFolderDialog.ShowDialog() == DialogResult.OK)
            {
                var path = modelFolderDialog.SelectedPath;
                Parameter = path.Import<Parameter>();
                LDAModel = path.Import<LDAModel>();
                ModelHelper.ImportVoca(path);

                UpdateTopicGridView();
            }
        }

        private void UpdateTopicGridView()
        {
            var topicTable = new DataTable();
            foreach (var topicId in Enumerable.Range(0, Parameter.TopicCount))
            {
                topicTable.Columns.Add(string.Format("Topic {0}", topicId));
                topicTable.Columns.Add(string.Format("Prob {0}", topicId));
            }

            foreach (var top in Enumerable.Range(0, 100))
            {
                var row = topicTable.NewRow();
                topicTable.Rows.Add(row);
            }

            foreach (var topicId in Enumerable.Range(0, Parameter.TopicCount))
            {
                var topicCol = string.Format("Topic {0}", topicId);
                var probCol = string.Format("Prob {0}", topicId);

                var wordDist = LDAModel.Phi[topicId]
                    .Select((e, wordId) => new { WordId = wordId, Prob = e })
                    .OrderByDescending(e => e.Prob)
                    .ToList();

                foreach (var top in Enumerable.Range(0, 100))
                {
                    var word = WordManager.ToWord(wordDist[top].WordId);
                    var prob = wordDist[top].Prob;
                    topicTable.Rows[top][topicCol] = word;
                    topicTable.Rows[top][probCol] = prob;
                }
            }

            topicGridView.DataSource = topicTable;
        }
    }
}
