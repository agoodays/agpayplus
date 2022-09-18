using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace AGooday.AgPay.Common.Utils
{
    public class TreeDataBuilder
    {
        /** 所有数据集合 **/
        private JArray nodes { get; set; }

        /** 默认数据中的主键key */
        private string idName = "id";

        /** 默认数据中的父级id的key */
        private string pidName = "pid";

        /** 默认数据中的子类对象key   */
        private string childrenName = "children";

        /** 排序字段， 默认按照ID排序 **/
        private string sortName = "id";

        /** 默认按照升序排序 **/
        private bool isAscSort = true;

        public TreeDataBuilder(JArray nodes)
        {
            this.nodes = nodes;
        }

        public TreeDataBuilder(JArray nodes, string idName, string pidName, string childrenName)
        {
            this.nodes = nodes;
            this.idName = idName;
            this.sortName = idName;  //排序字段，按照idName
            this.pidName = pidName;
            this.childrenName = childrenName;
        }

        /// <summary>
        /// 自定义字段 + 排序标志
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="idName"></param>
        /// <param name="pidName"></param>
        /// <param name="childrenName"></param>
        /// <param name="sortName"></param>
        /// <param name="isAscSort"></param>
        public TreeDataBuilder(JArray nodes, string idName, string pidName, string childrenName, string sortName, bool isAscSort)
        {
            this.nodes = nodes;
            this.idName = idName;
            this.pidName = pidName;
            this.childrenName = childrenName;
            this.sortName = sortName;
            this.isAscSort = isAscSort;
        }

        /// <summary>
        /// 构建JSON树形结构
        /// </summary>
        /// <returns></returns>
        public string BuildTreeString()
        {
            List<JObject> nodeTree = BuildTreeObject();
            return JsonConvert.SerializeObject(nodeTree);
        }

        // 构建树形结构
        public List<JObject> BuildTreeObject()
        {

            //定义待返回的对象
            List<JObject> resultNodes = new List<JObject>();

            //获取所有的根节点 （考虑根节点有多个的情况， 将根节点单独处理）
            List<JObject> rootNodes = GetRootNodes();

            ListSort(rootNodes); //排序

            //遍历根节点对象
            foreach (JObject rootNode in rootNodes)
            {

                BuildChildNodes(rootNode); //递归查找子节点并设置

                resultNodes.Add(rootNode); //添加到对象信息
            }
            return resultNodes;
        }

        /** 递归查找并赋值子节点 **/
        private void BuildChildNodes(JObject node)
        {
            List<JObject> children = GetChildNodes(node);
            if (children.Count() > 0)
            {
                foreach (JObject child in children)
                {
                    BuildChildNodes(child);
                }

                ListSort(children); //排序
                node.Add(childrenName, JArray.FromObject(children));
            }
        }

        /** 查找当前节点的子节点 */
        private List<JObject> GetChildNodes(JToken currentNode)
        {
            List<JObject> childNodes = new List<JObject>();
            foreach (JObject n in nodes)
            {
                if (currentNode[idName].Equals(n[pidName]))
                {
                    childNodes.Add(n);
                }
            }
            return childNodes;
        }

        /** 判断是否为根节点 */
        private bool IsRootNode(JObject node)
        {
            bool isRootNode = true;
            foreach (JObject n in nodes)
            {
                if (node[pidName] != null && node[pidName].Equals(n[idName]))
                {
                    isRootNode = false;
                    break;
                }
            }
            return isRootNode;
        }

        /** 获取集合中所有的根节点 */
        private List<JObject> GetRootNodes()
        {
            List<JObject> rootNodes = new List<JObject>();
            foreach (JObject n in nodes)
            {
                if (IsRootNode(n))
                {
                    rootNodes.Add(n);
                }
            }
            return rootNodes;
        }

        /** 将list进行排序  */
        private void ListSort(List<JObject> list)
        {
            list.Sort((o1, o2) =>
            {
                int result = 0;
                var o1v = o1.Value<string>(sortName);
                var o2v = o2.Value<string>(sortName);
                //if (o1v.All(char.IsDigit))
                //if(Regex.IsMatch(o1v, @"^[0-9]+$"))
                if (Int32.TryParse(o1v, out int _))
                {
                    result = Convert.ToInt32(o1v).CompareTo(Convert.ToInt32(o2v));
                }
                else if (o1v is not null)
                {
                    result = o1v.CompareTo(o2v);
                }

                if (!isAscSort)
                {
                    //倒序， 取反数
                    return -result;
                }

                return result;
            });
        }
    }
}
