using System.IO;
using System.Linq;
using System.Text;

namespace GridGameHOS.Minesweeper {
    public struct LayoutSetting {
        public int RowSize;
        public int ColumnSize;
        public int MineSize;
        public string LayoutDataArray;
        public LayoutSetting(int rowSize, int columnSize, int mineSize, string layoutDataArray) {
            RowSize = rowSize;
            ColumnSize = columnSize;
            MineSize = mineSize;
            LayoutDataArray = layoutDataArray;
        }
    }
    public static class GameLayoutLoader {
        public static LayoutSetting ReadFromFile(string filePath) {
            if (Directory.Exists(filePath)) {
                return new LayoutSetting(0, 0, 0, "");
            }
            string fileContent;
            using (StreamReader reader = new StreamReader(filePath)) {
                fileContent = reader.ReadToEnd();
            }
            string[] layouContent = fileContent.Split('\n');
            int rowSize = layouContent.Length;
            int columnSize = layouContent[0].Count((char ch) => (ch == '1' || ch == '0'));
            int mineSize = fileContent.Count((char ch) => ch == '1');
            if (rowSize == 0 || columnSize == 0 || mineSize == 0) {
                return new LayoutSetting(0, 0, 0, "");
            }
            StringBuilder sb = new StringBuilder();
            foreach (char ch in fileContent) {
                if (ch == '1' || ch == '0') {
                    sb.Append(ch);
                }
            }
            string layoutDataArray = sb.ToString();
            return new LayoutSetting(rowSize, columnSize, mineSize, layoutDataArray);
        }
    }
}
