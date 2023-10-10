using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Common.Utils
{
    /// <summary>
    /// [ 通用树状结构构造器 ]
    /// 解决： 将数据库查询到的多行List， 转换为层级关系的树状结构。
    /// 使用方式：
    ///     1. 先将查询的到对象List转换为JSONObject List，
    ///        在转换过程中JSONObject中必须包含 [id, pid](字段名称可自定义) 【！！必须是String类型！！】 ；
    ///     2. 使用构造函数创建对象，参数为转换好的对象， 如果自定义字段key 则将字段名称一并传入；
    ///     3. 使用buildTreeString() 或者 buildTreeObject() 生成所需对象；
    /// </summary>
    public class TreeDataBuilder
    {
        /// <summary>
        /// 所有数据集合
        /// </summary>
        private JArray nodes { get; set; }

        /// <summary>
        /// 默认数据中的主键key
        /// </summary>
        private string idName = "id";

        /// <summary>
        /// 默认数据中的父级id的key
        /// </summary>
        private string pidName = "pid";

        /// <summary>
        /// 默认数据中的子类对象key
        /// </summary>
        private string childrenName = "children";

        /// <summary>
        /// 排序字段， 默认按照ID排序
        /// </summary>
        private string sortName = "id";

        /// <summary>
        /// 默认按照升序排序
        /// </summary>
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

        /// <summary>
        /// 递归查找并赋值子节点
        /// </summary>
        /// <param name="node"></param>
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

        /// <summary>
        /// 查找当前节点的子节点
        /// </summary>
        /// <param name="currentNode"></param>
        /// <returns></returns>
        private List<JObject> GetChildNodes(JObject currentNode)
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

        /// <summary>
        /// 判断是否为根节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 获取集合中所有的根节点
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 将list进行排序
        /// </summary>
        /// <param name="list"></param>
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
