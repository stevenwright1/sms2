using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using AuthGateway.Shared;
using AuthGateway.Shared.XmlMessages.Response.Ret.AuthEngine;

namespace AuthGateway.AdminGUI
{
    public partial class frmChooseImage : Form
    {
        private List<string> imageCategories = new List<string>() { "people", "nature", "flowers", "animals", "business", "technology", "cars", "city", "christmas" };

        private Dictionary<string, string[]> categorizedPhotos = new Dictionary<string, string[]>();

        private Variables clientLogic;                
        public string SelectedImageUrl { get; private set; }

        public Bitmap SelectedImage { get; private set; }
        public frmChooseImage(Variables clientLogic)
        {            
            InitializeComponent();

            this.clientLogic = clientLogic;

            lbCategories.Items.AddRange(imageCategories.ToArray());

            llPexels.Links[0].LinkData = "https://www.pexels.com";            
        }

        private Tuple<int, int> GetCellByIndex(int index)
        {
            int columnsCount = dgvImages.Columns.Count;
            int ratio = index / columnsCount;
            int remainder = index % columnsCount;            
            return new Tuple<int, int>(ratio, remainder);
        }

        private int GetIndexByCell(Tuple<int, int> coords)
        {
            return coords.Item1 * dgvImages.Columns.Count + coords.Item2;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (dgvImages.CurrentCell != null) {
                string category = lbCategories.SelectedItem.ToString();
                int index = GetIndexByCell(new Tuple<int, int>(dgvImages.CurrentCell.RowIndex, dgvImages.CurrentCell.ColumnIndex));
                SelectedImageUrl = categorizedPhotos[category][index];                
            }
        }
        private Bitmap RetrieveImage(string url)
        {
            GetImageRet ret = null;
            int attempts = 3; // try next attempt in case of decrypt error
            for (int i = 0; i < attempts; i++) {
                try {
                    ret = clientLogic.GetImage(url);
                    break;
                }
                catch {
                }
            }

            Bitmap bmp = null;
            if (string.IsNullOrEmpty(ret.Error)) {
                byte[] buffer = ret.ImageBytes;
                bmp = ImagingHelper.GetImageFromBytes(buffer);
            }
            return bmp;
        }                

        private void AddImagesToGrid(string category, Bitmap[] images)
        {
            int rowsCount = (int)Math.Ceiling((double)images.Length / dgvImages.Columns.Count);
            int oldRowsCount = dgvImages.Rows.Count;
            try {
                dgvImages.Rows.Add(rowsCount);
            }
            catch (Exception ex) { }

            int startCell = oldRowsCount * dgvImages.Columns.Count;
            for (int i = 0; i < images.Length && category == lbCategories.SelectedItem.ToString(); i++) {
                Tuple<int, int> coords = GetCellByIndex(startCell + i);
                DataGridViewImageCell cell = (DataGridViewImageCell)dgvImages.Rows[coords.Item1].Cells[coords.Item2];
                if (images[i] != null) {
                    cell.Value = images[i];
                }
                else {
                    cell.Value = Properties.Resources.ErrorUploading;
                }
            }

            dgvImages.Update();
        }
        

        private string[] GetImagesByCategory(string category)
        {
            string[] urls = null;
            int attempts = 10; // try next attempt in case of decrypt error
            for (int i = 0; i < attempts; i++) {
                try {
                    GetImagesByCategoryRet ret = clientLogic.GetImagesByCategory(category);
                    urls = ret.Urls;
                    break;
                }
                catch { }
            }            
            return urls;
        }
        
        private void lbCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            try {
                if (lbCategories.SelectedIndex >= 0) {                                        
                    string category = lbCategories.SelectedItem.ToString();                    

                    BackgroundWorker bwLoadImages1 = new BackgroundWorker();                    
                    bwLoadImages1.WorkerReportsProgress = true;
                    bwLoadImages1.WorkerSupportsCancellation = true;
                    bwLoadImages1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwLoadImages_DoWork);
                    bwLoadImages1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bwLoadImages_ProgressChanged);
                    bwLoadImages1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwLoadImages_RunWorkerCompleted);
                    bwLoadImages1.RunWorkerAsync(category);                    
                }                
            }
            catch (Exception ex) {
                MessageBox.Show("Error receiving images.");
            }
        }

        private void dgvImages_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvImages.SelectedCells.Count > 0) {
                btnOK.Enabled = true;                
                dgvImages.CurrentCell.Style = new DataGridViewCellStyle { SelectionBackColor = System.Drawing.Color.Navy };

                int index = GetIndexByCell(new Tuple<int, int>(dgvImages.CurrentCellAddress.X, dgvImages.CurrentCellAddress.Y));
                string category = lbCategories.SelectedItem.ToString();
                SelectedImageUrl = categorizedPhotos[category][index];
                SelectedImage = (Bitmap)(dgvImages.CurrentCell).Value;
            }
            else {
                btnOK.Enabled = false;
                SelectedImageUrl = null;
            }
        }

        private void llPexels_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)        
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        private void LoadImages(string category, BackgroundWorker bw)
        {            
            try {                                             
                    bw.ReportProgress(0);
                    if (!categorizedPhotos.ContainsKey(category)) {
                        categorizedPhotos[category] = GetImagesByCategory(category);
                        if (categorizedPhotos[category] == null)
                            throw new Exception("Failed to get images.");
                    }

                    if (categorizedPhotos[category].Length == 0) {
                        MessageBox.Show(string.Format("Images for category '{0}' were not loaded yet. Please try again in a few minutes.", category));
                    }

                    int portionSize = 30;
                    int portionsCount = (int)Math.Ceiling((double)categorizedPhotos[category].Length / portionSize);
                    for (int i = 0; i < portionsCount && category == lbCategories.SelectedItem.ToString(); i++) {                        
                        int elementsNum = i == portionsCount - 1 ? categorizedPhotos[category].Length % portionSize : portionSize;
                        string[] portion = new string[elementsNum];
                        Array.Copy(categorizedPhotos[category], i * portionSize, portion, 0, elementsNum);
                        Bitmap[] images;
                        images = new Bitmap[portion.Length];
                        for (int j = 0; j < portion.Length && category == lbCategories.SelectedItem.ToString(); j++) {
                            try {
                                images[j] = RetrieveImage(portion[j]);
                            }
                            catch { }

                            //if (image == null) {
                            //    byte[] bytes;
                            //    image = (Bitmap)ImagingHelper.GetImageFromUrl(url, out bytes);
                            //    StoreImage(url, category,  /*image*/ bytes);
                            //}                
                        }  
                        bw.ReportProgress(1, new Tuple<string, Bitmap[]>(category, images));
                    }
            }
            catch (Exception ex) {
                MessageBox.Show(string.Format("Error receiving images for category '{0}'.", category));
            }
        }
        private void bwLoadImages_DoWork(object sender, DoWorkEventArgs e)
        {
            string category = e.Argument.ToString();
            BackgroundWorker bw = sender as BackgroundWorker;
            LoadImages(category, bw);
        }

        private void bwLoadImages_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {           
                if (e.ProgressPercentage == 0) {
                    dgvImages.Rows.Clear();
                }
                else {
                    Tuple<string, Bitmap[]> tuple = e.UserState as Tuple<string, Bitmap[]>;
                    AddImagesToGrid(tuple.Item1, tuple.Item2);
                }         
        }

        private void bwLoadImages_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }

        
    }
}
