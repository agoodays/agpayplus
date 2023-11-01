
#####  ↓↓↓↓↓↓↓↓↓↓  系统配置初始化DML  ↓↓↓↓↓↓↓↓↓↓  #####

UPDATE `t_sys_config` SET `group_key` = 'ossConfig' WHERE `config_key` = 'ossPublicSiteUrl' AND `group_key` = 'applicationConfig'
INSERT INTO `t_sys_config` VALUES ('apiMapWebKey', '[高德地图商户端web配置]Key', '高德地图Key', 'apiMapConfig', '高德地图商户端web配置', '6cebea39ba50a4c9bc565baaf57d1c8b', 'text', 0, '2023-02-11 18:30:00');
INSERT INTO `t_sys_config` VALUES ('apiMapWebSecret', '[高德地图商户端web配置]秘钥', '高德地图Key', 'apiMapConfig', '高德地图商户端web配置', 'dccbb5a56d2a1850eda2b6e67f8f2f13', 'text', 0, '2023-02-11 18:30:00');
INSERT INTO `t_sys_config` VALUES ('apiMapWebServiceKey', '[高德地图商户端web服务]Key', '商户端web服务key', 'apiMapConfig', '高德地图商户端web配置', '1e558c3dc1ce7ab2a0b332d78fcd4c16', 'text', 0, '2023-02-11 18:30:00');
INSERT INTO `t_sys_config` VALUES ('ossUseType', '文件上传服务类型', '文件上传服务类型', 'ossConfig', '文件上传服务', 'localFile', 'radio', 0, '2023-02-11 18:30:00');
INSERT INTO `t_sys_config` VALUES ('aliyunOssConfig', '阿里云oss配置', '阿里云oss配置', 'ossConfig', '阿里云oss配置', '{"accessKeyId":"LTAI4GEqjdMVqr6y7xTjsTo1","endpoint":"oss-cn-beijing.aliyuncs.com","expireTime":30000,"publicBucketName":"jeepaypublic","privateBucketName":"jeepayprivate"}', 'text', 0, '2023-02-11 18:30:00');
INSERT INTO `t_sys_config` VALUES ('agentSiteUrl', '代理商平台网址(不包含结尾/)', '代理商平台网址(不包含结尾/)', 'applicationConfig', '系统应用配置', 'https://localhost:9816', 'text', 0, '2023-02-11 18:30:00');
INSERT INTO `t_sys_config` VALUES ('mchPrivacyPolicy', '隐私政策', '隐私政策', 'mchTreatyConfig', '隐私政策', '<h2 style="text-indent: 0px; text-align: start;">隐私政策</h2><p style="text-indent: 0px; text-align: start;">计全商户通APP（以下简称“我们”）尊重并保护所有计全商户通用户的个人信息及隐私安全。为了给您提供更准确、更有个性化的服务，我们依据《中华人民共和国网络安全法》、《信息安全技术&nbsp;个人信息安全规范》以及其他相关法律法规和技术规范明确了我们收集/使用/披露您的个人信息的原则。本隐私政策进一步阐述了关于您个人信息的相关权利。</p><p style="text-indent: 0px; text-align: start;">本政策与您所使用的我们的产品与/或服务息息相关，请在使用/继续使用我们的各项产品与服务前，仔细阅读并充分理解本政策。如果您不同意本政策的内容，将可能导致我们的产品/服务无法正常运行，您应立即停止访问/使用我们的产品与/或服务。若您同意本《隐私政策》，即视为您已经仔细阅读/充分理解，并同意本《隐私政策》的全部内容。若您使用/继续使用我们提供的产品与/或服务的行为，均视为您已仔细阅读/充分理解，并同意本隐私政策的全部内容。</p><p style="text-indent: 0px; text-align: start;">本隐私政策属于计全商户通APP产品与/或服务使用协议不可分割的一部分。</p><h3 style="text-indent: 0px; text-align: start;">一、我们如何收集和使用您的个人信息</h3><p style="text-indent: 0px; text-align: start;">我们会遵循正当、合法、必要的原则，处于对本政策所述的以下目的，收集和使用您在使用我们的产品与/或服务时所主动提供，或因使用我们的产品与/或服务时被动产生的个人信息。除本政策另有规定外，在未征得您事先许可的情况下，我们不会将这些信息对外披露或提供给第三方。若我们需要将您的个人信息用于本政策未载明的其他用途，或基于特定目的将已经收集的信息用于其他目的，我们将以合理的方式告知您，并在使用前征得您的同意。</p><h4 style="text-indent: 0px; text-align: start;">1.账号注册及登录</h4><p style="text-indent: 0px; text-align: start;">1.1当您注册计全商户通APP账号时，您需要根据计全商户通APP的要求提供您的个人注册信息，我们会收集您所填写的商户名称、手机号码以及您所选择的商户类型。</p><p style="text-indent: 0px; text-align: start;">1.2为了给您提供更合理的服务，当您登录计全商户通APP时，我们会使用您的用户ID/手机号，以确认您账号所属的商户信息。</p><h4 style="text-indent: 0px; text-align: start;">2.向您提供产品与/或服务时</h4><p style="text-indent: 0px; text-align: start;">2.1信息浏览、管理、修改、新增等功能。</p><p style="text-indent: 0px; text-align: start;">当您使用计全商户通APP中的信息浏览、管理、修改和新增等功能时，我们会请求您授权照片、相机、和存储功能的权限。如果您拒绝授权提供，将无法使用相应的功能，但并不影响您使用计全商户通APP的其他功能。</p><p style="text-indent: 0px; text-align: start;">2.1.1当您使用用户头像修改/上传等功能时，我们会请求您授权存储功能的权限，如果您拒绝授权提供，将无法使用相应功能。但并不影响您使用计全商户通APP的其他功能。</p><p style="text-indent: 0px; text-align: start;">2.1.2当您使用计全商户通APP中的编辑个人信息、门店管理、码牌管理、云喇叭管理、云打印机管理等功能时，您所提供的图片、文字、状态等信息将会上传并存储至云端服务器中，由于存储是实现以上功能及其多端同步的必要条件。我们会以加密的方式存储这些信息，您也可以随时修改这些信息。</p><p style="text-indent: 0px; text-align: start;">2.2安全运行。为了保障软件与服务的安全运行，我们会收集您的设备型号、设备名称、设备唯一标识符（包括：IMEI、IMSI、Android&nbsp;ID、IDFA）、浏览器类型和设置、使用的语言、操作系统和应用程序版本、网络设备硬件地址、访问日期和时间、登录IP地址、接入网络的方式等。</p><p style="text-indent: 0px; text-align: start;">2.3搜索功能。当您使用计全商户通APP提供的搜索服务时，我们会收集您所输入的关键词信息、访问时间信息。这些信息是搜索功能的必要条件。</p><p style="text-indent: 0px; text-align: start;">2.4扫码。当您使用计全商户通APP提供的扫一扫支付、绑定新码、扫码获取云喇叭/打印机设备号等功能和/或服务时，我们会请求您授权相机的权限。如果您拒绝授权提供，将无法使用上述功能。</p><p style="text-indent: 0px; text-align: start;">2.5收款。当您使用计全商户通APP提供的收款功能时、我们会收集该笔收款订单的订单号、收款金额、收款时间、支付时间、支付方式、订单状态、门店信息、用户信息，这些信息用于生成详细的订单记录。</p><p style="text-indent: 0px; text-align: start;">2.6退款。当您使用计全商户通APP提供的订单退款功能时、我们会收集该笔订单的订单号、订单金额、支付金额、退款金额、支付时间、退款时间、门店信息、用户信息，这些信息用于生成详细的退款记录，并对比收退款金额，以限制退款金额不能大于支付金额。</p><p style="text-indent: 0px; text-align: start;">2.7查询。当您使用计全商户通APP提供的订单记录、门店列表、码牌列表、云喇叭列表、云打印机列表、收款通知接收人列表等功能时，我们会收集您的账户信息和商户ID，用于展示在您查询权限内的信息。</p><p style="text-indent: 0px; text-align: start;">2.8拨号。当您使用计全商户通APP，关于我们-联系电话中的快捷拨号功能时，我们会请求您设备的拨号权限。如果您拒绝授权提供，将无法使用快捷拨号功能，但不会影响您使用计全商户通APP的其他服务。</p><p style="text-indent: 0px; text-align: start;">2.9设备权限调用汇总</p><p style="text-indent: 0px; text-align: start;">我们对计全商户通APP为您提供服务时，所需要您授权的设备权限汇总如下。注意：您可以拒绝其中部分权限，但将无法使用需要该权限的功能和服务。您可以随时取消已授权的设备权限，不同设备权限显示方式和关闭方式可能有所不同，具体请参考设备及操作系统开发方的说明和操作指引：</p><table style="text-align: start;"><tbody><tr><td colspan="1" rowspan="1"><strong>设备权限</strong></td><td colspan="1" rowspan="1"><strong>对应业务功能</strong></td><td colspan="1" rowspan="1"><strong>功能场景说明</strong></td><td colspan="1" rowspan="1"><strong>取消授权</strong></td></tr><tr><td colspan="1" rowspan="1">相机</td><td colspan="1" rowspan="1">扫一扫收款与喇叭/云打印机新增、修改绑定新码</td><td colspan="1" rowspan="1">1.扫描消费者付款码，使用相机识别二维码。&nbsp;2.扫描未绑定的码牌时，使用相机识别二维码。3.获取云打印机/云喇叭设备号时，使用相机识别设备条码</td><td colspan="1" rowspan="1">​该权限允许拒绝或取消授权，不影响APP其他功能</td></tr><tr><td colspan="1" rowspan="1">访问照片</td><td colspan="1" rowspan="1">头像上传通道申请上传照片</td><td colspan="1" rowspan="1">1.用户头像上传时我们需要访问您的相册，以便您选取要上传的照片2.通道申请时上传企业营业执照、法人身份证照片、商户门头照等信息时，我们需要访问您的相册，以便您选取要上传的照片</td><td colspan="1" rowspan="1">该权限允许拒绝或取消授权，不影响APP其他功能</td></tr><tr><td colspan="1" rowspan="1">储存</td><td colspan="1" rowspan="1">APP稳定运行下载码牌图片</td><td colspan="1" rowspan="1">1.日志信息记录、信息缓存2.下载码牌图片至手机相册中</td><td colspan="1" rowspan="1">该权限允许拒绝或取消授权，不影响APP其他功能</td></tr><tr><td colspan="1" rowspan="1">获取电话状态（设备信息）</td><td colspan="1" rowspan="1">APP安全运行</td><td colspan="1" rowspan="1">1.本政策第2.2条描述</td><td colspan="1" rowspan="1">该权限允许拒绝或取消授权，不影响APP其他功能</td></tr></tbody></table><h4 style="text-indent: 0px; text-align: start;">3.征得授权同意的例外</h4><p style="text-indent: 0px; text-align: start;">根据相关法律法规的规定，在以下情形中，我们可以在不征得您的授权同意的情况下收集、使用一些必要的个人信息：</p><p style="text-indent: 0px; text-align: start;">（1）与我们旅行法律法规规定的义务相关的；</p><p style="text-indent: 0px; text-align: start;">（2）与国家安全、国防安全直接相关的；</p><p style="text-indent: 0px; text-align: start;">（3）与公共安全、公共卫生、重大公共利益直接相关的；</p><p style="text-indent: 0px; text-align: start;">（4）与犯罪侦查、起诉、审判和判决执行及相关事项直接相关的；</p><p style="text-indent: 0px; text-align: start;">（5）出于维护您或其他个人的生命、财产及相关重大合法权益但有很难得到本人同意的；</p><p style="text-indent: 0px; text-align: start;">（6）所收集的个人信息是您自行向社会公众公开的；</p><p style="text-indent: 0px; text-align: start;">（7）从合法公开披露的信息中收集到您的客人信息：从合法的新闻报道、政府信息公开等相关渠道；</p><p style="text-indent: 0px; text-align: start;">（8）根据您与平台签署的在线协议或合同所必需的；</p><p style="text-indent: 0px; text-align: start;">（9）用于维护我们产品和/或服务的安全稳定运行所必需的：发现、处置产品或服务的故障及相关问题处理；</p><p style="text-indent: 0px; text-align: start;">（10）法律法规规定的其他情形。</p><h3 style="text-indent: 0px; text-align: start;">二、我们如何共享、转让、公开披露您的个人信息</h3><h4 style="text-indent: 0px; text-align: start;">1.共享</h4><p style="text-indent: 0px; text-align: start;">对于您的个人信息，我们不会与任何公司、组织和个人进行共享，除非存在以下一种或多种情形：</p><p style="text-indent: 0px; text-align: start;">（1）事先已获得您的授权；</p><p style="text-indent: 0px; text-align: start;">（2）您自行提出的；</p><p style="text-indent: 0px; text-align: start;">（3）与商业合作伙伴的必要共享：</p><p style="text-indent: 0px; text-align: start;">您理解并知悉，为了向您提供更完善、优质的产品和服务；或由于您在使用计全商户通中由第三方服务提供企业/机构所提供的服务时的情况下，我们将授权第三方服务提供企业/机构为您提供部分服务。此种情况下，我们可能会与合作伙伴共享您的某些个人信息，其中包括您已授权或自行提出的（包括但不限于商户名称、手机号、法人信息、商户营业执照等）必要信息，以及您在使用本APP时自动产生的某些信息（包括订单、订单金额、交易时间、收款方式、收款金额、门店信息、退款信息）。请您注意、我们仅处于合法、正当、必要、特定、明确的目的共享这些信息。我们将对信息数据的输出形式、流转、使用进行安全评估与处理，以保护数据安全。同时，我们会对合作伙伴、服务商机构进行严格的监督与管理，一但发现其存在违规处理个人信息的行为，将立即停止合作并追究其法律责任。</p><p style="text-indent: 0px; text-align: start;">目前，我们的合作伙伴包括以下类型：</p><p style="text-indent: 0px; text-align: start;">A.第三方支付机构：当您使用计全商户通提供的支付业务时，将会使用并通过第三方支付机构的支付通道，其中包括但不限于微信支付、支付宝支付、银联云闪付支付等第三方支付平台。我们会与第三方支付机构共享来自于您的部分交易信息。为保障您在使用我们所提供的收款功能/服务时的合理、合规及合法性，在您正式使用前述功能/服务前，您需要向对应的第三方支付机构发起支付通道申请，在此情况下，我们会收集您所主动提供的商户名称、企业名称、法人信息、营业执照、账户信息等必要信息，并将上述信息与第三方支付机构共享。</p><p style="text-indent: 0px; text-align: start;">（4）您可以基于计全商户通APP与第三人（包括不特定对象）共享您的个人信息或其他信息，但因您的共享行为而导致的信息泄露、被使用及其他相关请何况，与计全商户通APP无关，计全商户通不因此承担法律责任。</p><h4 style="text-indent: 0px; text-align: start;">2.转让</h4><p style="text-indent: 0px; text-align: start;">转让是指将取得您个人信息的控制权转让给其他公司、组织或个人。除非获取您的明确同意，否则我们不会将您的个人信息转让给任何公司、组织或个人。但下述情形除外：</p><p style="text-indent: 0px; text-align: start;">（1）已事先征得您的同意；</p><p style="text-indent: 0px; text-align: start;">（2）您自行提出的；</p><p style="text-indent: 0px; text-align: start;">（3）如果公司发生合并、收购或破产清算，将可能涉及到个人信息转让，此种情况下我们会告知您有关情况并要求新的持有您个人信息的公司、组织继续受本政策的约束。否则我们将要求其重新获取您的明示同意。</p><p style="text-indent: 0px; text-align: start;">（4）其他法律法规规定的情形。</p><h4 style="text-indent: 0px; text-align: start;">3.公开披露</h4><p style="text-indent: 0px; text-align: start;">公开披露是指向社会或不特定人群发布信息的行为。原则上，我们不会对您的个人信息进行公开披露。但下述情况除外：</p><p style="text-indent: 0px; text-align: start;">（1）取得您的明示同意后。</p><h4 style="text-indent: 0px; text-align: start;">4.共享、转让、公开披露个人信息授权同意的例外情形</h4><p style="text-indent: 0px; text-align: start;">根据相关法律法规的固定，在以下情形中，我们可能在未事先征得您的授权同意的情况下共享、转让、公开披露您的个人信息：</p><p style="text-indent: 0px; text-align: start;">（1）与我们履行法律法规规定的义务相关的，含依照法律规定、司法机关或行政机关强制要求向有权机关披露您的个人信息；在该种情况下，我们会要求披露请求方出局与其请求相应的有效法律文件，并对被披露的信息采取符合法律和业界标准的安全防护措施；</p><p style="text-indent: 0px; text-align: start;">（2）与国家安全、国防安全直接相关的；</p><p style="text-indent: 0px; text-align: start;">（3）与公共安全、公共卫生、重大公共利益直接相关的；</p><p style="text-indent: 0px; text-align: start;">（4）与犯罪侦擦、&nbsp;起诉、审判和判决执行及相关事项直接相关的；</p><p style="text-indent: 0px; text-align: start;">（5）出于维护您或其他个人的生命、财产及相关重大合法权益但又很难得到本人同意的；</p><p style="text-indent: 0px; text-align: start;">（6）您自行向社会公众公开的个人信息；</p><p style="text-indent: 0px; text-align: start;">（7）从合法公开披露的信息中收集到的个人信息：合法的新闻报道、政府信息公开及其他相关渠道；</p><p style="text-indent: 0px; text-align: start;">（8）法律法规规定的其他情形。</p><p style="text-indent: 0px; text-align: start;">请您了解，根据现行法律规定及监管要求，共享、转让经去标识化处理的个人信息，且确保数据接收方无法复原并重新识别个人信息主体的，无需另行向您通知并征得您的同意。</p><h3 style="text-indent: 0px; text-align: start;">三、我们如何存储和保护您的个人信息</h3><h4 style="text-indent: 0px; text-align: start;">1.存储</h4><p style="text-indent: 0px; text-align: start;">存储地点：我们将从中华人民共和国境内获得的个人信息存放于中华人民共和国境内。如果发生个人信息的跨境传输，我们会单独向您以邮件或通知的方式告知您个人信息处境的目的与接受方，并征得您的同意。我们会严格遵守中华人民共和国法律法规，确保数据接收方有充足的数据能力来保护您的个人信息安全。</p><p style="text-indent: 0px; text-align: start;">存储时间：我们承诺始终按照法律的规定在合理必要期限内存储您的个人信息。超出上述期限后，我们将删除您的个人信息或对您的个人信息进行脱敏、去标识化、匿名化处理。</p><p style="text-indent: 0px; text-align: start;">如果我们停止运营，我们将立即停止收集您的个人信息，并将停止运营这一决定和/或事实以右键或通知的方式告知您，并对所有已获取的您的个人信息进行删除或匿名化处理。</p><h4 style="text-indent: 0px; text-align: start;">2.保护</h4><p style="text-indent: 0px; text-align: start;">为了您的个人信息安全，我们将采用各种符合行业标准的安全措施来保护您的个人信息安全，以最大程度降低您的个人信息被损毁、泄露、盗用、非授权方式访问、使用、披露和更改的风险。我们将积极建立数据分类分级制度、数据安全管理规范、数据安全开发规范来管理规范个人信息的采集、存储和使用，确保未收集与我们提供的产品和服务无关的信息。</p><p style="text-indent: 0px; text-align: start;">在数据存储安全上，我们与第三方机构合作，包括但不限于阿里云、腾讯云等。</p><p style="text-indent: 0px; text-align: start;">为保障您的账户和个人信息的安全，请妥善保管您的账户及密码信息。我们将通过多端服务器备份的方式保障您的信息不丢失、损毁或因不可抗力因素而导致的无法使用。通过第三方存储服务机构提供的堡垒机、安全防火墙等服务，保障您的信息不被滥用、变造和泄露。</p><p style="text-indent: 0px; text-align: start;">尽管有上述安全措施，但也请您注意：在信息网络上并不存在绝对“完善的安全措施”。为防止安全事故的发生，我们已按照法律法规规定，制定了妥善的预警机制和应急预案。如确发生安全事件，我们将及时将相关情况以邮件、电话、信函、短信等方式告知您、难以逐一告知单一个体时，我们会采取合理、有效的方式发布公告。同时，我们还将按照监管部门要求，主动上报个人信息安全事件的处置情况、紧密配合政府机关的工作。</p><p style="text-indent: 0px; text-align: start;">当我们的产品或服务发生停止运营的情况下，我们将立即停止收集您的个人信息，并将停止运营这一决定和/或事实以邮件或通知的方式告知您，并对所有已获取的您的个人信息进行删除或匿名化处理。</p><h4 style="text-indent: 0px; text-align: start;">3.匿名化处理</h4><p style="text-indent: 0px; text-align: start;">为保障我们已收集到的您的个人信息的安全，当我们不再提供服务、停止运营或因其他不可抗力因素而不得不销毁您的个人信息的情况下。我们将会采取删除或匿名化的方式处理您的个人信息。</p><p style="text-indent: 0px; text-align: start;">匿名化是指通过对个人信息的技术处理，使个人信息的主体无法被识别，且处理后无法被复原的过程。严格意义上，匿名化后的信息不属于个人信息。</p><h3 style="text-indent: 0px; text-align: start;">四、您如何管理您的个人信息</h3><h4 style="text-indent: 0px; text-align: start;">1.自主决定授权信息</h4><p style="text-indent: 0px; text-align: start;">您可以自主决定是否授权我们向您申请的某些设备权限，具体请参考第一条，2.9所述。</p><p style="text-indent: 0px; text-align: start;">注意：根据不同的操作系统及硬件设备，您管理这些权限的方式可能会有所不同，具体操作方式，请参照您的设备或操作系统开发方的说明和操作指引。</p><h4 style="text-indent: 0px; text-align: start;">2.访问、获取、更改和删除相关信息</h4><p style="text-indent: 0px; text-align: start;">您可以通过交互本APP的交互界面对相关信息进行访问、获取、更改和删除。</p><p style="text-indent: 0px; text-align: start;">（1）您登录账户的名称和头像：</p><p style="text-indent: 0px; text-align: start;">您可以通过在“我的”页面，通过点击头像一栏的按钮进入修改个人信息页面，对个人名称进行查看和修改。通过点击个人头像，查看您的账户头像，授权我们访问您的相册后，您可以更改您的账户头像。</p><p style="text-indent: 0px; text-align: start;">（2）门店信息：</p><p style="text-indent: 0px; text-align: start;">您可以在“首页-门店管理”页面，通过点击门店列表中的单个门店进入所选门店的编辑页面，对门店名称和备注信息进行查看和修改。通过点击状态条目中的开关按钮，您可以启用或禁用所选门店的状态。</p><p style="text-indent: 0px; text-align: start;">（3）云喇叭/云打印机设备信息：</p><p style="text-indent: 0px; text-align: start;">您可以在“我的-云喇叭管理/云打印管理”页面，通过点击设备列表中的单个云喇叭/云打印机进入所选设备的编辑页面，对设备名称、设备编号、所属门店等信息进行查看和修改。通过点击状态条目中的开关按钮，您可以启用或禁用所选设备的状态。</p><h3 style="text-indent: 0px; text-align: start;">五、您如何注销您的账号</h3><p style="text-indent: 0px; text-align: start;">您可以通过第九条中指明的联系方式联系我们，并像我们阐明您注销账号的原因。或在本APP的”我的-设置-其他设置-注销账号“页面输入注销原因并点击提交按钮向我们提交您的注销申请。在满足账号注销的条件下，我们将尽快注销您的账号。注意：由于您账号在使用期间内产生的交易信息将不会被立刻处理，而是需要经过确认、复查后，确保该笔交易已完成所有流程后，进行脱敏处理。此外，除法律明确规定必须由我们保留的个人信息外，您在使用本APP期间内所产生或由您提交的其他个人信息将会被删除或匿名化处理，且该处理不可逆，您将无法找回这些个人信息。</p><h3 style="text-indent: 0px; text-align: start;">六、有关第三方提供产品和/或服务的特别说明</h3><p style="text-indent: 0px; text-align: start;">您在使用计全商户通APP时，可能会使用到由第三方提供的产品和/或服务，在这种情况下，您需要接受该第三方的服务条款及隐私政策（而非本隐私政策）的约束，您需要仔细阅读其条款并自行决定是否接受。请您妥善保管您的个人信息，仅在必要的情况下向他人提供。本政策仅适用于我们所收集、保存、使用、共享、披露信息，并不适用于任何第三方提供服务时（包含您向该第三方提供的任何个人信息）或第三方信息的使用规则，第三方使用您的个人信息时的行为，由其自行负责。</p><h3 style="text-indent: 0px; text-align: start;">七、我们如何使用Cookie和其他同类技术</h3><p style="text-indent: 0px; text-align: start;">在您未拒绝接受cookies的情况下，我们会在您的计算机以及相关移动设备上设定或取用cookies，以便您能登录或使用依赖于cookies的计全商户通的产品与/或服务。您有权选择接受或拒绝接受cookies。您可以通过修改浏览器设置的方式或在移动设备设置中设置拒绝我们使用cookies。若您拒绝使用cookies，则您可能无法登录或使用依赖于cookies的计全商户通App网络服务或功能。</p><h3 style="text-indent: 0px; text-align: start;">八、更新隐私政策</h3><p style="text-indent: 0px; text-align: start;">我们保留更新或修订本隐私政策的权力。这些修订或更新构成本政策的一部分，并具有等同于本政策的效。未经您的同意，我们不会削减您依据当前生效的本政策所应享受的权利。</p><p style="text-indent: 0px; text-align: start;">我们会不时更新本政策，如遇本政策更新，我们会通过APP通知等相关合理方式通知您，如遇重大更新，您需要重新仔细阅读、充分理解并同意修订更新后的政策，才可继续使用我们所提供的产品和/或服务。</p><h3 style="text-indent: 0px; text-align: start;">九、联系我们</h3><p style="text-indent: 0px; text-align: start;">如果您对本政策有任何疑问，您可以通过以下方式联系我们，我们将尽快审核所涉问题，并在验证您的用户身份后予以答复。</p><p style="text-indent: 0px; text-align: start;">网站：<a href="https://www.jeequan.com" target="_blank">www.jeequan.com&nbsp;&nbsp;&nbsp;</a></p><h3 style="text-indent: 0px; text-align: start;">十、其他</h3><p style="text-indent: 0px; text-align: start;">如果您认为我们的个人信息处理行为损害了您的合法权益，您可以向有关政府部门进行反应。或因本政策以及我们处理您个人信息事宜引起的任何争议，您可以诉至沧州市人民法院。</p>', 'editor', 0, '2023-02-11 18:30:00');
INSERT INTO `t_sys_config` VALUES ('mchServiceAgreement', '用户服务协议', '用户服务协议', 'mchTreatyConfig', '用户服务协议', '<h2 style="text-indent: 0px; text-align: start;">用户服务协议</h2><p style="text-indent: 0px; text-align: start;">感谢您使用计全支付，在使用“计全商户通”软件及相关服务前，请您认真阅读本协议，并确认承诺同意遵守本协议的全部约定。本协议由您与计全科技（河北）有限公司（包括其关联机构，以下合成“本公司”）于您点击同意本协议之时，在河北省沧州市签署并生效。</p><h3 style="text-indent: 0px; text-align: start;">一、协议条款的确认及接受</h3><p style="text-indent: 0px; text-align: start;"><strong>1.</strong>计全支付（包括网址为<a href="https://www.jeequan.com" target="_blank">www.jeepay.com&nbsp;</a>的网站，以及可在IOS系统及Android&nbsp;系统中运行的名为“计全商户通APP”、“计全展业宝APP”、及其他不同版本的应用程序，以及名为“计全商户通”、“计全展业宝”的微信小程序，以下简称"本网站"或“计全支付”）由计全科技（河北）有限公司（包括其关联机构，以下合称“本公司”）运营并享有完全的所有权及知识产权等权益，计全支付提供的服务将完全按照其发布的条款和操作规则严格执行。</p><p style="text-indent: 0px; text-align: start;"><strong>2.</strong>您确认同意本协议（协议文本包括《计全支付用户服务协议》、《计全支付用户隐私政策》及计全支付已公示或将来公示的各项规则及提示，所有前述协议、规则及提示乃不可分割的整体，具有同等法律效力，共同构成用户使用计全支付及相关服务的整体协议，以下合称“本协议”）所有条款并完成注册程序时，本协议在您于本公司间成立并发生法律效力，同时您成为计全支付正式用户。</p><h3 style="text-indent: 0px; text-align: start;">二、账号注册及使用规则</h3><p style="text-indent: 0px; text-align: start;"><strong>1.</strong>当您使用“计全商户通”APP时，您可以在APP/微信小程序中的注册页，或在地址为<a href="https://mch.s.jeepay.vip/register" target="_blank">https://mch.s.jeepay.vip/register</a>的网页进行注册，注册成功后，计全支付将给与您一个商户账号及相应的密码，该商户账号和密码有您负责保管，您应当对以其商户账号进行的所有活动和事件负法律责任。</p><p style="text-indent: 0px; text-align: start;"><strong>2.</strong>您须对在计全支付注册信息的真实性、合法性、有效性承担全部责任，您不得冒充他人（包括但不限于冒用他人姓名、名称、字号、头像等足以让人引起混淆的方式，以及冒用他人手机号码）开设账号；不得利用他人的名义发布任何信息；不得利用他人的名义发起任何交易；不得恶意使用注册账户导致他人误认；否则计全支付有权立即停止提供服务，收回账号，并由您独自承担由此而产生的一切法律责任。</p><p style="text-indent: 0px; text-align: start;"><strong>3.</strong>您理解且确认，您在计全支付注册的账号的所有权及有关权益均归本公司所有，您完成注册手续后仅享有该账号的使用权（包括但不限于该账号绑定的由计全支付提供的产品和/或服务）。您的账号仅限于您本人使用，未经本公司书面同意，禁止以任何形式赠与、借用、出租、转让、售卖或以其他任何形式许可他人使用该账号。如果本公司发现或有合理理由认为使用者并非账号初始注册人，公司有权在未通知您的请款修改，暂停或终止向该账号提供服务，并有权注销该账号，而无需向注册该账号的用户承担法律责任。</p><p style="text-indent: 0px; text-align: start;"><strong>4.</strong>您理解并确认，除用于登录、使用“计全商户通”APP及相关服务外，您还可以使用您的注册账号登录使用计全支付提供的其他商户产品和/或服务，以及其他本公司的合作伙伴、第三方服务提供商所提供的软件及服务。若您以计全支付账号登录和/或使用前述服务时，同样应受到其他软件、服务实际提供方的用户协议及其他协议条款的约束。</p><p style="text-indent: 0px; text-align: start;"><strong>5.</strong>您理解并确认，部分由其他第三方平台（包括但不限于银联云闪付、微信支付、支付宝支付、随行付等）提供的产品及服务，在您使用计全支付提供的产品和/或服务时，仅作为基础服务为您提供。您的计全支付账号与您在上述第三方平台注册的第三方平台账号仅在技术层面上构成单方面绑定。如果您不使用/不继续使用计全支付提供的产品和/或服务，您的第三方平台账号均不会受到影响，您可以继续第三方平台提供的产品及服务。</p><p style="text-indent: 0px; text-align: start;"><strong>6.</strong>您承诺不得以任何方式利用计全支付直接或间接从事违反中国法律、社会公德的行为，计全支付有权对违反上述承诺的内容予以屏蔽、留证，并将由您独自承担由此而产生的一切法律责任。</p><p style="text-indent: 0px; text-align: start;"><strong>7.</strong>您不得利用本网站制作、上载、复制、发布、传播或转载如下内容：</p><p style="text-indent: 0px; text-align: start;">（1）反对宪法所确定的基本原则的；</p><p style="text-indent: 0px; text-align: start;">（2）危害国家安全，泄露国家秘密，颠覆国家政权，破坏国家统一的；</p><p style="text-indent: 0px; text-align: start;">（3）损害国家荣誉和利益的；</p><p style="text-indent: 0px; text-align: start;">（4）煽动民族仇恨、民族歧视，破坏民族团结的；</p><p style="text-indent: 0px; text-align: start;">（5）破坏国家宗教政策，宣扬邪教和封建迷信的；</p><p style="text-indent: 0px; text-align: start;">（6）散布谣言，扰乱社会秩序，破坏社会稳定的；</p><p style="text-indent: 0px; text-align: start;">（7）散布淫秽、色情、赌博、暴力、凶杀、恐怖或者教唆犯罪的；</p><p style="text-indent: 0px; text-align: start;">（8）侮辱或者诽谤他人，侵害他人合法权益的；</p><p style="text-indent: 0px; text-align: start;">（9）侵害未成年人合法权益或者损害未成年人身心健康的；</p><p style="text-indent: 0px; text-align: start;">（10）含有法律、行政法规禁止的其他内容的信息。</p><p style="text-indent: 0px; text-align: start;"><strong>8.</strong>计全支付有权对您使用我们的产品和/或服务时的情况进行审查和监督，如您在使用计全支付时违反任何上述规定，本公司有权暂停或终止对您提供服务，以减轻您的不当行为所造成的影响。</p><h3 style="text-indent: 0px; text-align: start;">三、服务内容</h3><p style="text-indent: 0px; text-align: start;"><strong>1.</strong>本公司可能为不同的终端设备及使用需求开发不同的应用程序软件版本，您应当更具实际设备需求状况获取、下载、安装合适的版本。</p><p style="text-indent: 0px; text-align: start;"><strong>2.</strong>本网站的服务具体内容根据实际情况提供，计全支付保留变更、终端或终止部分或全部服务的权力。计全支付不承担因业务调整给您造成的损失。除非本协议另有其他明示规定，增加或强化目前本网站的任何新功能，包括所推出的新产品，均受到本协议之规范。您了解并同意，本网站服务仅依其当前所呈现的状况提供，对于任何用户通讯或个人化设定之时效、删除、传递错误、未予储存或其他任何问题，计全支付均不承担任何责任。如您不接受服务调整，请停止使用本服务；否则，您的继续使用行为将被视为其对调整服务的完全同意且承诺遵守。</p><p style="text-indent: 0px; text-align: start;"><strong>3.</strong>计全支付在提供服务时，&nbsp;可能会对部分服务的用户收取一定的费用或交易佣金。在此情况下，计全支付会在相关页面上做明确的提示。如您拒绝支付该等费用，则不能使用相关的服务。</p><p style="text-indent: 0px; text-align: start;"><strong>4.</strong>您理解，计全支付仅提供相应的服务，除此外与相关服务有关的设备（如电脑、移动设备、调制解调器及其他与接入互联网有关的装置）及所需的费用（如电话费、上网费等）均应由您自行负担。</p><p style="text-indent: 0px; text-align: start;"><strong>5.</strong>计全支付提供的服务可能包括：文字、软件、声音、图片、视频、数据统计、图表、支付通道等。所有这些内容均受著作权、商标和其他财产所有权法律保护。您只有在获得计全支付或其他相关权利人的授权之后才能使用这些内容，不能擅自复制、再造这些内容、或创造与内容有关的派生产品。</p><h3 style="text-indent: 0px; text-align: start;">四、知识产权</h3><p style="text-indent: 0px; text-align: start;"><strong>1.</strong>本公司在计全支付软件及相关服务中提供的内容（包括但不限于软件、技术、程序、网页、文字、图片、图像、商标、标识、音频、视频、图表、版面设计、电子文档等，未申明版权或网络上公开的无版权内容除外）的知识产权属于本公司所有。同时本公司提供服务所依托的软件著作权、专利权、商标及其他知识产权均归本公司所有。未经本公司许可，任何人不得擅自使用。</p><p style="text-indent: 0px; text-align: start;"><strong>2.</strong>请您在任何情况下都不要私自使用本公司的包括但不限于“计全”、“计全支付”、“Jeepay”、“jeepay.cn”、“jeepay.com”、“jeequan”和“jeequan.com”等在内的任何商标、服务标记、商号、域名、网站名称或其他显著品牌特征等（以下统称为“标识”）。未经本公司事先书面同意，您不得将本条款前述标识以单独或结合任何方式展示、使用或申请注册商标、进行域名注册等，也不得实时向他人明示或暗示有权展示、使用或其他有权处理这些标识的行为。由于您违反本协议使用公司上述商标、标识等给本公司或他人造成损失的，由您承担全部法律责任。</p><h3 style="text-indent: 0px; text-align: start;">五、用户授权及隐私保护</h3><p style="text-indent: 0px; text-align: start;">计全支付尊重并保护所有计全支付用户的个人信息及隐私安全。为了给您提供更准确、更有个性化的服务，计全支付依据《中华人民共和国网络安全法》、《信息安全技术&nbsp;个人信息安全规范》以及其他相关法律法规和技术规范明确了本公司收集/使用/披露您的个人信息的原则。详情请参照<a href="https://uutool.cn/ueditor/#%E9%9A%90%E7%A7%81%E6%94%BF%E7%AD%96" target="">《隐私协议》</a>。</p><h3 style="text-indent: 0px; text-align: start;">六、免责声明</h3><p style="text-indent: 0px; text-align: start;"><strong>1.</strong>计全支付不担保本网站服务一定能满足您的要求，也不担保本网站服务不会中断，对本网站服务的及时性、安全性、准确性、真实性、完整性也都不作担保。</p><p style="text-indent: 0px; text-align: start;"><strong>2.</strong>对于因不可抗力或计全支付不能控制的原因造成的本网站服务终端或其他缺陷，本网站不承担任何责任，但本公司将尽力减少因此而给您造成的损失和影响。</p><p style="text-indent: 0px; text-align: start;"><strong>3.</strong>对于您利用计全支付或本公司发布的其他产品和/或服务，进行违法犯罪，或进行任何违反中国法律、社会公德的行为，本公司有权立即停止对您提供服务，并将由您独自承担由此产生的一切法律责任。</p><h3 style="text-indent: 0px; text-align: start;">七、违约责任</h3><p style="text-indent: 0px; text-align: start;"><strong>1.</strong>针对您违反本协议或其他服务条款的行为，本公司有权独立判断并视情况采取预先警示、限制帐号部分或者全部功能直至永久关闭帐号等措施。本公司有权公告处理结果，且有权根据实际情况决定是否恢复使用。对涉嫌违反法律法规、涉嫌违法犯罪的行为将保存有关记录，并依法向有关主管部门报告、配合有关主管部门调查。</p><h3 style="text-indent: 0px; text-align: start;">八、协议修改</h3><p style="text-indent: 0px; text-align: start;"><strong>1.</strong>计全支付有权根据法律法规政策、国家有权机构或公司经营要求修改本协议的有关条款，计全支付会通过适当的方式在网站上予以公示。</p><p style="text-indent: 0px; text-align: start;"><strong>2.</strong>若您不同意计全支付对本协议相关条款所做的修改，您有权停止使用本网站服务。如果您继续使用本网站服务，则视为您接受计全支付对本协议相关条款所做的修改。</p>', 'editor', 0, '2023-02-11 18:30:00');
INSERT INTO `t_sys_config` VALUES ('agentPrivacyPolicy', '隐私政策', '隐私政策', 'agentTreatyConfig', '隐私政策', '<h2 style="text-indent: 0px; text-align: start;">隐私政策</h2><p style="text-indent: 0px; text-align: start;">计全展业宝APP（以下简称“我们”）尊重并保护所有计全商户通用户的个人信息及隐私安全。为了给您提供更准确、更有个性化的服务，我们依据《中华人民共和国网络安全法》、《信息安全技术&nbsp;个人信息安全规范》以及其他相关法律法规和技术规范明确了我们收集/使用/披露您的个人信息的原则。本隐私政策进一步阐述了关于您个人信息的相关权利。</p><p style="text-indent: 0px; text-align: start;">本政策与您所使用的我们的产品与/或服务息息相关，请在使用/继续使用我们的各项产品与服务前，仔细阅读并充分理解本政策。如果您不同意本政策的内容，将可能导致我们的产品/服务无法正常运行，您应立即停止访问/使用我们的产品与/或服务。若您同意本《隐私政策》，即视为您已经仔细阅读/充分理解，并同意本《隐私政策》的全部内容。若您使用/继续使用我们提供的产品与/或服务的行为，均视为您已仔细阅读/充分理解，并同意本隐私政策的全部内容。</p><p style="text-indent: 0px; text-align: start;">本隐私政策属于计全展业宝APP产品与/或服务使用协议不可分割的一部分。</p><h3 style="text-indent: 0px; text-align: start;">一、我们如何收集和使用您的个人信息</h3><p style="text-indent: 0px; text-align: start;">我们会遵循正当、合法、必要的原则，处于对本政策所述的以下目的，收集和使用您在使用我们的产品与/或服务时所主动提供，或因使用我们的产品与/或服务时被动产生的个人信息。除本政策另有规定外，在未征得您事先许可的情况下，我们不会将这些信息对外披露或提供给第三方。若我们需要将您的个人信息用于本政策未载明的其他用途，或基于特定目的将已经收集的信息用于其他目的，我们将以合理的方式告知您，并在使用前征得您的同意。</p><h4 style="text-indent: 0px; text-align: start;">1.账号注册及登录</h4><p style="text-indent: 0px; text-align: start;">1.1当您注册计全展业宝APP账号时，您需要根据计全展业宝APP的要求提供您的个人注册信息，我们会收集您所填写的商户名称、手机号码以及您所选择的商户类型。</p><p style="text-indent: 0px; text-align: start;">1.2为了给您提供更合理的服务，当您登录计全展业宝APP时，我们会使用您的用户ID/手机号，以确认您账号所属的商户信息。</p><h4 style="text-indent: 0px; text-align: start;">2.向您提供产品与/或服务时</h4><p style="text-indent: 0px; text-align: start;">2.1信息浏览、管理、修改、新增等功能。</p><p style="text-indent: 0px; text-align: start;">当您使用计全展业宝APP中的信息浏览、管理、修改和新增等功能时，我们会请求您授权照片、相机、和存储功能的权限。如果您拒绝授权提供，将无法使用相应的功能，但并不影响您使用计全商户通APP的其他功能。</p><p style="text-indent: 0px; text-align: start;">2.1.1当您使用用户头像修改/上传等功能时，我们会请求您授权存储功能的权限，如果您拒绝授权提供，将无法使用相应功能。但并不影响您使用计全展业宝APP的其他功能。</p><p style="text-indent: 0px; text-align: start;">2.1.2当您使用计全展业宝APP中的编辑个人信息、门店管理、码牌管理、云喇叭管理、云打印机管理等功能时，您所提供的图片、文字、状态等信息将会上传并存储至云端服务器中，由于存储是实现以上功能及其多端同步的必要条件。我们会以加密的方式存储这些信息，您也可以随时修改这些信息。</p><p style="text-indent: 0px; text-align: start;">2.2安全运行。为了保障软件与服务的安全运行，我们会收集您的设备型号、设备名称、设备唯一标识符（包括：IMEI、IMSI、Android&nbsp;ID、IDFA）、浏览器类型和设置、使用的语言、操作系统和应用程序版本、网络设备硬件地址、访问日期和时间、登录IP地址、接入网络的方式等。</p><p style="text-indent: 0px; text-align: start;">2.3搜索功能。当您使用计全展业宝APP提供的搜索服务时，我们会收集您所输入的关键词信息、访问时间信息。这些信息是搜索功能的必要条件。</p><p style="text-indent: 0px; text-align: start;">2.4扫码。当您使用计全展业宝APP提供的扫一扫支付、绑定新码、扫码获取云喇叭/打印机设备号等功能和/或服务时，我们会请求您授权相机的权限。如果您拒绝授权提供，将无法使用上述功能。</p><p style="text-indent: 0px; text-align: start;">2.5收款。当您使用计全展业宝APP提供的收款功能时、我们会收集该笔收款订单的订单号、收款金额、收款时间、支付时间、支付方式、订单状态、门店信息、用户信息，这些信息用于生成详细的订单记录。</p><p style="text-indent: 0px; text-align: start;">2.6退款。当您使用计全展业宝APP提供的订单退款功能时、我们会收集该笔订单的订单号、订单金额、支付金额、退款金额、支付时间、退款时间、门店信息、用户信息，这些信息用于生成详细的退款记录，并对比收退款金额，以限制退款金额不能大于支付金额。</p><p style="text-indent: 0px; text-align: start;">2.7查询。当您使用计全展业宝APP提供的订单记录、门店列表、码牌列表、云喇叭列表、云打印机列表、收款通知接收人列表等功能时，我们会收集您的账户信息和商户ID，用于展示在您查询权限内的信息。</p><p style="text-indent: 0px; text-align: start;">2.8拨号。当您使用计全展业宝APP，关于我们-联系电话中的快捷拨号功能时，我们会请求您设备的拨号权限。如果您拒绝授权提供，将无法使用快捷拨号功能，但不会影响您使用计全展业宝APP的其他服务。</p><p style="text-indent: 0px; text-align: start;">2.9设备权限调用汇总</p><p style="text-indent: 0px; text-align: start;">我们对计全展业宝APP为您提供服务时，所需要您授权的设备权限汇总如下。注意：您可以拒绝其中部分权限，但将无法使用需要该权限的功能和服务。您可以随时取消已授权的设备权限，不同设备权限显示方式和关闭方式可能有所不同，具体请参考设备及操作系统开发方的说明和操作指引：</p><table style="text-align: start;"><tbody><tr><td colspan="1" rowspan="1"><strong>设备权限</strong></td><td colspan="1" rowspan="1"><strong>对应业务功能</strong></td><td colspan="1" rowspan="1"><strong>功能场景说明</strong></td><td colspan="1" rowspan="1"><strong>取消授权</strong></td></tr><tr><td colspan="1" rowspan="1">相机</td><td colspan="1" rowspan="1">扫一扫收款与喇叭/云打印机新增、修改绑定新码</td><td colspan="1" rowspan="1">1.扫描消费者付款码，使用相机识别二维码。&nbsp;2.扫描未绑定的码牌时，使用相机识别二维码。3.获取云打印机/云喇叭设备号时，使用相机识别设备条码</td><td colspan="1" rowspan="1">​该权限允许拒绝或取消授权，不影响APP其他功能</td></tr><tr><td colspan="1" rowspan="1">访问照片</td><td colspan="1" rowspan="1">头像上传通道申请上传照片</td><td colspan="1" rowspan="1">1.用户头像上传时我们需要访问您的相册，以便您选取要上传的照片2.通道申请时上传企业营业执照、法人身份证照片、商户门头照等信息时，我们需要访问您的相册，以便您选取要上传的照片</td><td colspan="1" rowspan="1">该权限允许拒绝或取消授权，不影响APP其他功能</td></tr><tr><td colspan="1" rowspan="1">储存</td><td colspan="1" rowspan="1">APP稳定运行下载码牌图片</td><td colspan="1" rowspan="1">1.日志信息记录、信息缓存2.下载码牌图片至手机相册中</td><td colspan="1" rowspan="1">该权限允许拒绝或取消授权，不影响APP其他功能</td></tr><tr><td colspan="1" rowspan="1">获取电话状态（设备信息）</td><td colspan="1" rowspan="1">APP安全运行</td><td colspan="1" rowspan="1">1.本政策第2.2条描述</td><td colspan="1" rowspan="1">该权限允许拒绝或取消授权，不影响APP其他功能</td></tr></tbody></table><h4 style="text-indent: 0px; text-align: start;">3.征得授权同意的例外</h4><p style="text-indent: 0px; text-align: start;">根据相关法律法规的规定，在以下情形中，我们可以在不征得您的授权同意的情况下收集、使用一些必要的个人信息：</p><p style="text-indent: 0px; text-align: start;">（1）与我们旅行法律法规规定的义务相关的；</p><p style="text-indent: 0px; text-align: start;">（2）与国家安全、国防安全直接相关的；</p><p style="text-indent: 0px; text-align: start;">（3）与公共安全、公共卫生、重大公共利益直接相关的；</p><p style="text-indent: 0px; text-align: start;">（4）与犯罪侦查、起诉、审判和判决执行及相关事项直接相关的；</p><p style="text-indent: 0px; text-align: start;">（5）出于维护您或其他个人的生命、财产及相关重大合法权益但有很难得到本人同意的；</p><p style="text-indent: 0px; text-align: start;">（6）所收集的个人信息是您自行向社会公众公开的；</p><p style="text-indent: 0px; text-align: start;">（7）从合法公开披露的信息中收集到您的客人信息：从合法的新闻报道、政府信息公开等相关渠道；</p><p style="text-indent: 0px; text-align: start;">（8）根据您与平台签署的在线协议或合同所必需的；</p><p style="text-indent: 0px; text-align: start;">（9）用于维护我们产品和/或服务的安全稳定运行所必需的：发现、处置产品或服务的故障及相关问题处理；</p><p style="text-indent: 0px; text-align: start;">（10）法律法规规定的其他情形。</p><h3 style="text-indent: 0px; text-align: start;">二、我们如何共享、转让、公开披露您的个人信息</h3><h4 style="text-indent: 0px; text-align: start;">1.共享</h4><p style="text-indent: 0px; text-align: start;">对于您的个人信息，我们不会与任何公司、组织和个人进行共享，除非存在以下一种或多种情形：</p><p style="text-indent: 0px; text-align: start;">（1）事先已获得您的授权；</p><p style="text-indent: 0px; text-align: start;">（2）您自行提出的；</p><p style="text-indent: 0px; text-align: start;">（3）与商业合作伙伴的必要共享：</p><p style="text-indent: 0px; text-align: start;">您理解并知悉，为了向您提供更完善、优质的产品和服务；或由于您在使用计全展业宝中由第三方服务提供企业/机构所提供的服务时的情况下，我们将授权第三方服务提供企业/机构为您提供部分服务。此种情况下，我们可能会与合作伙伴共享您的某些个人信息，其中包括您已授权或自行提出的（包括但不限于商户名称、手机号、法人信息、商户营业执照等）必要信息，以及您在使用本APP时自动产生的某些信息（包括订单、订单金额、交易时间、收款方式、收款金额、门店信息、退款信息）。请您注意、我们仅处于合法、正当、必要、特定、明确的目的共享这些信息。我们将对信息数据的输出形式、流转、使用进行安全评估与处理，以保护数据安全。同时，我们会对合作伙伴、服务商机构进行严格的监督与管理，一但发现其存在违规处理个人信息的行为，将立即停止合作并追究其法律责任。</p><p style="text-indent: 0px; text-align: start;">目前，我们的合作伙伴包括以下类型：</p><p style="text-indent: 0px; text-align: start;">A.第三方支付机构：当您使用计全展业宝提供的支付业务时，将会使用并通过第三方支付机构的支付通道，其中包括但不限于微信支付、支付宝支付、银联云闪付支付等第三方支付平台。我们会与第三方支付机构共享来自于您的部分交易信息。为保障您在使用我们所提供的收款功能/服务时的合理、合规及合法性，在您正式使用前述功能/服务前，您需要向对应的第三方支付机构发起支付通道申请，在此情况下，我们会收集您所主动提供的商户名称、企业名称、法人信息、营业执照、账户信息等必要信息，并将上述信息与第三方支付机构共享。</p><p style="text-indent: 0px; text-align: start;">（4）您可以基于计全展业宝APP与第三人（包括不特定对象）共享您的个人信息或其他信息，但因您的共享行为而导致的信息泄露、被使用及其他相关请何况，与计全展业宝APP无关，计全展业宝不因此承担法律责任。</p><h4 style="text-indent: 0px; text-align: start;">2.转让</h4><p style="text-indent: 0px; text-align: start;">转让是指将取得您个人信息的控制权转让给其他公司、组织或个人。除非获取您的明确同意，否则我们不会将您的个人信息转让给任何公司、组织或个人。但下述情形除外：</p><p style="text-indent: 0px; text-align: start;">（1）已事先征得您的同意；</p><p style="text-indent: 0px; text-align: start;">（2）您自行提出的；</p><p style="text-indent: 0px; text-align: start;">（3）如果公司发生合并、收购或破产清算，将可能涉及到个人信息转让，此种情况下我们会告知您有关情况并要求新的持有您个人信息的公司、组织继续受本政策的约束。否则我们将要求其重新获取您的明示同意。</p><p style="text-indent: 0px; text-align: start;">（4）其他法律法规规定的情形。</p><h4 style="text-indent: 0px; text-align: start;">3.公开披露</h4><p style="text-indent: 0px; text-align: start;">公开披露是指向社会或不特定人群发布信息的行为。原则上，我们不会对您的个人信息进行公开披露。但下述情况除外：</p><p style="text-indent: 0px; text-align: start;">（1）取得您的明示同意后。</p><h4 style="text-indent: 0px; text-align: start;">4.共享、转让、公开披露个人信息授权同意的例外情形</h4><p style="text-indent: 0px; text-align: start;">根据相关法律法规的固定，在以下情形中，我们可能在未事先征得您的授权同意的情况下共享、转让、公开披露您的个人信息：</p><p style="text-indent: 0px; text-align: start;">（1）与我们履行法律法规规定的义务相关的，含依照法律规定、司法机关或行政机关强制要求向有权机关披露您的个人信息；在该种情况下，我们会要求披露请求方出局与其请求相应的有效法律文件，并对被披露的信息采取符合法律和业界标准的安全防护措施；</p><p style="text-indent: 0px; text-align: start;">（2）与国家安全、国防安全直接相关的；</p><p style="text-indent: 0px; text-align: start;">（3）与公共安全、公共卫生、重大公共利益直接相关的；</p><p style="text-indent: 0px; text-align: start;">（4）与犯罪侦擦、&nbsp;起诉、审判和判决执行及相关事项直接相关的；</p><p style="text-indent: 0px; text-align: start;">（5）出于维护您或其他个人的生命、财产及相关重大合法权益但又很难得到本人同意的；</p><p style="text-indent: 0px; text-align: start;">（6）您自行向社会公众公开的个人信息；</p><p style="text-indent: 0px; text-align: start;">（7）从合法公开披露的信息中收集到的个人信息：合法的新闻报道、政府信息公开及其他相关渠道；</p><p style="text-indent: 0px; text-align: start;">（8）法律法规规定的其他情形。</p><p style="text-indent: 0px; text-align: start;">请您了解，根据现行法律规定及监管要求，共享、转让经去标识化处理的个人信息，且确保数据接收方无法复原并重新识别个人信息主体的，无需另行向您通知并征得您的同意。</p><h3 style="text-indent: 0px; text-align: start;">三、我们如何存储和保护您的个人信息</h3><h4 style="text-indent: 0px; text-align: start;">1.存储</h4><p style="text-indent: 0px; text-align: start;">存储地点：我们将从中华人民共和国境内获得的个人信息存放于中华人民共和国境内。如果发生个人信息的跨境传输，我们会单独向您以邮件或通知的方式告知您个人信息处境的目的与接受方，并征得您的同意。我们会严格遵守中华人民共和国法律法规，确保数据接收方有充足的数据能力来保护您的个人信息安全。</p><p style="text-indent: 0px; text-align: start;">存储时间：我们承诺始终按照法律的规定在合理必要期限内存储您的个人信息。超出上述期限后，我们将删除您的个人信息或对您的个人信息进行脱敏、去标识化、匿名化处理。</p><p style="text-indent: 0px; text-align: start;">如果我们停止运营，我们将立即停止收集您的个人信息，并将停止运营这一决定和/或事实以右键或通知的方式告知您，并对所有已获取的您的个人信息进行删除或匿名化处理。</p><h4 style="text-indent: 0px; text-align: start;">2.保护</h4><p style="text-indent: 0px; text-align: start;">为了您的个人信息安全，我们将采用各种符合行业标准的安全措施来保护您的个人信息安全，以最大程度降低您的个人信息被损毁、泄露、盗用、非授权方式访问、使用、披露和更改的风险。我们将积极建立数据分类分级制度、数据安全管理规范、数据安全开发规范来管理规范个人信息的采集、存储和使用，确保未收集与我们提供的产品和服务无关的信息。</p><p style="text-indent: 0px; text-align: start;">在数据存储安全上，我们与第三方机构合作，包括但不限于阿里云、腾讯云等。</p><p style="text-indent: 0px; text-align: start;">为保障您的账户和个人信息的安全，请妥善保管您的账户及密码信息。我们将通过多端服务器备份的方式保障您的信息不丢失、损毁或因不可抗力因素而导致的无法使用。通过第三方存储服务机构提供的堡垒机、安全防火墙等服务，保障您的信息不被滥用、变造和泄露。</p><p style="text-indent: 0px; text-align: start;">尽管有上述安全措施，但也请您注意：在信息网络上并不存在绝对“完善的安全措施”。为防止安全事故的发生，我们已按照法律法规规定，制定了妥善的预警机制和应急预案。如确发生安全事件，我们将及时将相关情况以邮件、电话、信函、短信等方式告知您、难以逐一告知单一个体时，我们会采取合理、有效的方式发布公告。同时，我们还将按照监管部门要求，主动上报个人信息安全事件的处置情况、紧密配合政府机关的工作。</p><p style="text-indent: 0px; text-align: start;">当我们的产品或服务发生停止运营的情况下，我们将立即停止收集您的个人信息，并将停止运营这一决定和/或事实以邮件或通知的方式告知您，并对所有已获取的您的个人信息进行删除或匿名化处理。</p><h4 style="text-indent: 0px; text-align: start;">3.匿名化处理</h4><p style="text-indent: 0px; text-align: start;">为保障我们已收集到的您的个人信息的安全，当我们不再提供服务、停止运营或因其他不可抗力因素而不得不销毁您的个人信息的情况下。我们将会采取删除或匿名化的方式处理您的个人信息。</p><p style="text-indent: 0px; text-align: start;">匿名化是指通过对个人信息的技术处理，使个人信息的主体无法被识别，且处理后无法被复原的过程。严格意义上，匿名化后的信息不属于个人信息。</p><h3 style="text-indent: 0px; text-align: start;">四、您如何管理您的个人信息</h3><h4 style="text-indent: 0px; text-align: start;">1.自主决定授权信息</h4><p style="text-indent: 0px; text-align: start;">您可以自主决定是否授权我们向您申请的某些设备权限，具体请参考第一条，2.9所述。</p><p style="text-indent: 0px; text-align: start;">注意：根据不同的操作系统及硬件设备，您管理这些权限的方式可能会有所不同，具体操作方式，请参照您的设备或操作系统开发方的说明和操作指引。</p><h4 style="text-indent: 0px; text-align: start;">2.访问、获取、更改和删除相关信息</h4><p style="text-indent: 0px; text-align: start;">您可以通过交互本APP的交互界面对相关信息进行访问、获取、更改和删除。</p><p style="text-indent: 0px; text-align: start;">（1）您登录账户的名称和头像：</p><p style="text-indent: 0px; text-align: start;">您可以通过在“我的”页面，通过点击头像一栏的按钮进入修改个人信息页面，对个人名称进行查看和修改。通过点击个人头像，查看您的账户头像，授权我们访问您的相册后，您可以更改您的账户头像。</p><p style="text-indent: 0px; text-align: start;">（2）门店信息：</p><p style="text-indent: 0px; text-align: start;">您可以在“首页-门店管理”页面，通过点击门店列表中的单个门店进入所选门店的编辑页面，对门店名称和备注信息进行查看和修改。通过点击状态条目中的开关按钮，您可以启用或禁用所选门店的状态。</p><p style="text-indent: 0px; text-align: start;">（3）云喇叭/云打印机设备信息：</p><p style="text-indent: 0px; text-align: start;">您可以在“我的-云喇叭管理/云打印管理”页面，通过点击设备列表中的单个云喇叭/云打印机进入所选设备的编辑页面，对设备名称、设备编号、所属门店等信息进行查看和修改。通过点击状态条目中的开关按钮，您可以启用或禁用所选设备的状态。</p><h3 style="text-indent: 0px; text-align: start;">五、您如何注销您的账号</h3><p style="text-indent: 0px; text-align: start;">您可以通过第九条中指明的联系方式联系我们，并像我们阐明您注销账号的原因。或在本APP的”我的-设置-其他设置-注销账号“页面输入注销原因并点击提交按钮向我们提交您的注销申请。在满足账号注销的条件下，我们将尽快注销您的账号。注意：由于您账号在使用期间内产生的交易信息将不会被立刻处理，而是需要经过确认、复查后，确保该笔交易已完成所有流程后，进行脱敏处理。此外，除法律明确规定必须由我们保留的个人信息外，您在使用本APP期间内所产生或由您提交的其他个人信息将会被删除或匿名化处理，且该处理不可逆，您将无法找回这些个人信息。</p><h3 style="text-indent: 0px; text-align: start;">六、有关第三方提供产品和/或服务的特别说明</h3><p style="text-indent: 0px; text-align: start;">您在使用计全展业宝APP时，可能会使用到由第三方提供的产品和/或服务，在这种情况下，您需要接受该第三方的服务条款及隐私政策（而非本隐私政策）的约束，您需要仔细阅读其条款并自行决定是否接受。请您妥善保管您的个人信息，仅在必要的情况下向他人提供。本政策仅适用于我们所收集、保存、使用、共享、披露信息，并不适用于任何第三方提供服务时（包含您向该第三方提供的任何个人信息）或第三方信息的使用规则，第三方使用您的个人信息时的行为，由其自行负责。</p><h3 style="text-indent: 0px; text-align: start;">七、我们如何使用Cookie和其他同类技术</h3><p style="text-indent: 0px; text-align: start;">在您未拒绝接受cookies的情况下，我们会在您的计算机以及相关移动设备上设定或取用cookies，以便您能登录或使用依赖于cookies的计全展业宝的产品与/或服务。您有权选择接受或拒绝接受cookies。您可以通过修改浏览器设置的方式或在移动设备设置中设置拒绝我们使用cookies。若您拒绝使用cookies，则您可能无法登录或使用依赖于cookies的计全展业宝App网络服务或功能。</p><h3 style="text-indent: 0px; text-align: start;">八、更新隐私政策</h3><p style="text-indent: 0px; text-align: start;">我们保留更新或修订本隐私政策的权力。这些修订或更新构成本政策的一部分，并具有等同于本政策的效。未经您的同意，我们不会削减您依据当前生效的本政策所应享受的权利。</p><p style="text-indent: 0px; text-align: start;">我们会不时更新本政策，如遇本政策更新，我们会通过APP通知等相关合理方式通知您，如遇重大更新，您需要重新仔细阅读、充分理解并同意修订更新后的政策，才可继续使用我们所提供的产品和/或服务。</p><h3 style="text-indent: 0px; text-align: start;">九、联系我们</h3><p style="text-indent: 0px; text-align: start;">如果您对本政策有任何疑问，您可以通过以下方式联系我们，我们将尽快审核所涉问题，并在验证您的用户身份后予以答复。</p><p style="text-indent: 0px; text-align: start;">网站：<a href="https://www.jeequan.com" target="_blank">www.jeequan.com</a></p><h3 style="text-indent: 0px; text-align: start;">十、其他</h3><p style="text-indent: 0px; text-align: start;">如果您认为我们的个人信息处理行为损害了您的合法权益，您可以向有关政府部门进行反应。或因本政策以及我们处理您个人信息事宜引起的任何争议，您可以诉至沧州市人民法院。</p>', 'editor', 0, '2023-02-11 18:30:00');
INSERT INTO `t_sys_config` VALUES ('agentServiceAgreement', '用户服务协议', '用户服务协议', 'agentTreatyConfig', '用户服务协议', '<h2 style="text-indent: 0px; text-align: start;">用户服务协议</h2><p style="text-indent: 0px; text-align: start;">感谢您使用计全支付，在使用“计全展业宝”软件及相关服务前，请您认真阅读本协议，并确认承诺同意遵守本协议的全部约定。本协议由您与计全科技（河北）有限公司（包括其关联机构，以下合成“本公司”）于您点击同意本协议之时，在河北省沧州市签署并生效。</p><h3 style="text-indent: 0px; text-align: start;">一、协议条款的确认及接受</h3><p style="text-indent: 0px; text-align: start;"><strong>1.</strong>计全支付（包括网址为<a href="https://www.jeequan.com" target="_blank">www.jeepay.com&nbsp;</a>的网站，以及可在IOS系统及Android&nbsp;系统中运行的名为“计全商户通APP”、“计全展业宝APP”、及其他不同版本的应用程序，以及名为“计全商户通”、“计全展业宝”的微信小程序，以下简称"本网站"或“计全支付”）由计全科技（河北）有限公司（包括其关联机构，以下合称“本公司”）运营并享有完全的所有权及知识产权等权益，计全支付提供的服务将完全按照其发布的条款和操作规则严格执行。</p><p style="text-indent: 0px; text-align: start;"><strong>2.</strong>您确认同意本协议（协议文本包括《计全支付用户服务协议》、《计全支付用户隐私政策》及计全支付已公示或将来公示的各项规则及提示，所有前述协议、规则及提示乃不可分割的整体，具有同等法律效力，共同构成用户使用计全支付及相关服务的整体协议，以下合称“本协议”）所有条款并完成注册程序时，本协议在您于本公司间成立并发生法律效力，同时您成为计全支付正式用户。</p><h3 style="text-indent: 0px; text-align: start;">二、账号注册及使用规则</h3><p style="text-indent: 0px; text-align: start;"><strong>1.</strong>当您使用“计全展业宝”APP时，您可以在APP/微信小程序中的注册页，或在地址为<a href="https://mch.s.jeepay.vip/register" target="_blank">https://mch.s.jeepay.vip/register</a>的网页进行注册，注册成功后，计全支付将给与您一个商户账号及相应的密码，该商户账号和密码有您负责保管，您应当对以其商户账号进行的所有活动和事件负法律责任。</p><p style="text-indent: 0px; text-align: start;"><strong>2.</strong>您须对在计全支付注册信息的真实性、合法性、有效性承担全部责任，您不得冒充他人（包括但不限于冒用他人姓名、名称、字号、头像等足以让人引起混淆的方式，以及冒用他人手机号码）开设账号；不得利用他人的名义发布任何信息；不得利用他人的名义发起任何交易；不得恶意使用注册账户导致他人误认；否则计全支付有权立即停止提供服务，收回账号，并由您独自承担由此而产生的一切法律责任。</p><p style="text-indent: 0px; text-align: start;"><strong>3.</strong>您理解且确认，您在计全支付注册的账号的所有权及有关权益均归本公司所有，您完成注册手续后仅享有该账号的使用权（包括但不限于该账号绑定的由计全支付提供的产品和/或服务）。您的账号仅限于您本人使用，未经本公司书面同意，禁止以任何形式赠与、借用、出租、转让、售卖或以其他任何形式许可他人使用该账号。如果本公司发现或有合理理由认为使用者并非账号初始注册人，公司有权在未通知您的请款修改，暂停或终止向该账号提供服务，并有权注销该账号，而无需向注册该账号的用户承担法律责任。</p><p style="text-indent: 0px; text-align: start;"><strong>4.</strong>您理解并确认，除用于登录、使用“计全展业宝”APP及相关服务外，您还可以使用您的注册账号登录使用计全支付提供的其他商户产品和/或服务，以及其他本公司的合作伙伴、第三方服务提供商所提供的软件及服务。若您以计全支付账号登录和/或使用前述服务时，同样应受到其他软件、服务实际提供方的用户协议及其他协议条款的约束。</p><p style="text-indent: 0px; text-align: start;"><strong>5.</strong>您理解并确认，部分由其他第三方平台（包括但不限于银联云闪付、微信支付、支付宝支付、随行付等）提供的产品及服务，在您使用计全支付提供的产品和/或服务时，仅作为基础服务为您提供。您的计全支付账号与您在上述第三方平台注册的第三方平台账号仅在技术层面上构成单方面绑定。如果您不使用/不继续使用计全支付提供的产品和/或服务，您的第三方平台账号均不会受到影响，您可以继续第三方平台提供的产品及服务。</p><p style="text-indent: 0px; text-align: start;"><strong>6.</strong>您承诺不得以任何方式利用计全支付直接或间接从事违反中国法律、社会公德的行为，计全支付有权对违反上述承诺的内容予以屏蔽、留证，并将由您独自承担由此而产生的一切法律责任。</p><p style="text-indent: 0px; text-align: start;"><strong>7.</strong>您不得利用本网站制作、上载、复制、发布、传播或转载如下内容：</p><p style="text-indent: 0px; text-align: start;">（1）反对宪法所确定的基本原则的；</p><p style="text-indent: 0px; text-align: start;">（2）危害国家安全，泄露国家秘密，颠覆国家政权，破坏国家统一的；</p><p style="text-indent: 0px; text-align: start;">（3）损害国家荣誉和利益的；</p><p style="text-indent: 0px; text-align: start;">（4）煽动民族仇恨、民族歧视，破坏民族团结的；</p><p style="text-indent: 0px; text-align: start;">（5）破坏国家宗教政策，宣扬邪教和封建迷信的；</p><p style="text-indent: 0px; text-align: start;">（6）散布谣言，扰乱社会秩序，破坏社会稳定的；</p><p style="text-indent: 0px; text-align: start;">（7）散布淫秽、色情、赌博、暴力、凶杀、恐怖或者教唆犯罪的；</p><p style="text-indent: 0px; text-align: start;">（8）侮辱或者诽谤他人，侵害他人合法权益的；</p><p style="text-indent: 0px; text-align: start;">（9）侵害未成年人合法权益或者损害未成年人身心健康的；</p><p style="text-indent: 0px; text-align: start;">（10）含有法律、行政法规禁止的其他内容的信息。</p><p style="text-indent: 0px; text-align: start;"><strong>8.</strong>计全支付有权对您使用我们的产品和/或服务时的情况进行审查和监督，如您在使用计全支付时违反任何上述规定，本公司有权暂停或终止对您提供服务，以减轻您的不当行为所造成的影响。</p><h3 style="text-indent: 0px; text-align: start;">三、服务内容</h3><p style="text-indent: 0px; text-align: start;"><strong>1.</strong>本公司可能为不同的终端设备及使用需求开发不同的应用程序软件版本，您应当更具实际设备需求状况获取、下载、安装合适的版本。</p><p style="text-indent: 0px; text-align: start;"><strong>2.</strong>本网站的服务具体内容根据实际情况提供，计全支付保留变更、终端或终止部分或全部服务的权力。计全支付不承担因业务调整给您造成的损失。除非本协议另有其他明示规定，增加或强化目前本网站的任何新功能，包括所推出的新产品，均受到本协议之规范。您了解并同意，本网站服务仅依其当前所呈现的状况提供，对于任何用户通讯或个人化设定之时效、删除、传递错误、未予储存或其他任何问题，计全支付均不承担任何责任。如您不接受服务调整，请停止使用本服务；否则，您的继续使用行为将被视为其对调整服务的完全同意且承诺遵守。</p><p style="text-indent: 0px; text-align: start;"><strong>3.</strong>计全支付在提供服务时，&nbsp;可能会对部分服务的用户收取一定的费用或交易佣金。在此情况下，计全支付会在相关页面上做明确的提示。如您拒绝支付该等费用，则不能使用相关的服务。</p><p style="text-indent: 0px; text-align: start;"><strong>4.</strong>您理解，计全支付仅提供相应的服务，除此外与相关服务有关的设备（如电脑、移动设备、调制解调器及其他与接入互联网有关的装置）及所需的费用（如电话费、上网费等）均应由您自行负担。</p><p style="text-indent: 0px; text-align: start;"><strong>5.</strong>计全支付提供的服务可能包括：文字、软件、声音、图片、视频、数据统计、图表、支付通道等。所有这些内容均受著作权、商标和其他财产所有权法律保护。您只有在获得计全支付或其他相关权利人的授权之后才能使用这些内容，不能擅自复制、再造这些内容、或创造与内容有关的派生产品。</p><h3 style="text-indent: 0px; text-align: start;">四、知识产权</h3><p style="text-indent: 0px; text-align: start;"><strong>1.</strong>本公司在计全支付软件及相关服务中提供的内容（包括但不限于软件、技术、程序、网页、文字、图片、图像、商标、标识、音频、视频、图表、版面设计、电子文档等，未申明版权或网络上公开的无版权内容除外）的知识产权属于本公司所有。同时本公司提供服务所依托的软件著作权、专利权、商标及其他知识产权均归本公司所有。未经本公司许可，任何人不得擅自使用。</p><p style="text-indent: 0px; text-align: start;"><strong>2.</strong>请您在任何情况下都不要私自使用本公司的包括但不限于“计全”、“计全支付”、“Jeepay”、“jeepay.cn”、“jeepay.com”、“jeequan”和“jeequan.com”等在内的任何商标、服务标记、商号、域名、网站名称或其他显著品牌特征等（以下统称为“标识”）。未经本公司事先书面同意，您不得将本条款前述标识以单独或结合任何方式展示、使用或申请注册商标、进行域名注册等，也不得实时向他人明示或暗示有权展示、使用或其他有权处理这些标识的行为。由于您违反本协议使用公司上述商标、标识等给本公司或他人造成损失的，由您承担全部法律责任。</p><h3 style="text-indent: 0px; text-align: start;">五、用户授权及隐私保护</h3><p style="text-indent: 0px; text-align: start;">计全支付尊重并保护所有计全支付用户的个人信息及隐私安全。为了给您提供更准确、更有个性化的服务，计全支付依据《中华人民共和国网络安全法》、《信息安全技术&nbsp;个人信息安全规范》以及其他相关法律法规和技术规范明确了本公司收集/使用/披露您的个人信息的原则。详情请参照<a href="https://uutool.cn/ueditor/#%E9%9A%90%E7%A7%81%E6%94%BF%E7%AD%96" target="">《隐私协议》</a>。</p><h3 style="text-indent: 0px; text-align: start;">六、免责声明</h3><p style="text-indent: 0px; text-align: start;"><strong>1.</strong>计全支付不担保本网站服务一定能满足您的要求，也不担保本网站服务不会中断，对本网站服务的及时性、安全性、准确性、真实性、完整性也都不作担保。</p><p style="text-indent: 0px; text-align: start;"><strong>2.</strong>对于因不可抗力或计全支付不能控制的原因造成的本网站服务终端或其他缺陷，本网站不承担任何责任，但本公司将尽力减少因此而给您造成的损失和影响。</p><p style="text-indent: 0px; text-align: start;"><strong>3.</strong>对于您利用计全支付或本公司发布的其他产品和/或服务，进行违法犯罪，或进行任何违反中国法律、社会公德的行为，本公司有权立即停止对您提供服务，并将由您独自承担由此产生的一切法律责任。</p><h3 style="text-indent: 0px; text-align: start;">七、违约责任</h3><p style="text-indent: 0px; text-align: start;"><strong>1.</strong>针对您违反本协议或其他服务条款的行为，本公司有权独立判断并视情况采取预先警示、限制帐号部分或者全部功能直至永久关闭帐号等措施。本公司有权公告处理结果，且有权根据实际情况决定是否恢复使用。对涉嫌违反法律法规、涉嫌违法犯罪的行为将保存有关记录，并依法向有关主管部门报告、配合有关主管部门调查。</p><h3 style="text-indent: 0px; text-align: start;">八、协议修改</h3><p style="text-indent: 0px; text-align: start;"><strong>1.</strong>计全支付有权根据法律法规政策、国家有权机构或公司经营要求修改本协议的有关条款，计全支付会通过适当的方式在网站上予以公示。</p><p style="text-indent: 0px; text-align: start;"><strong>2.</strong>若您不同意计全支付对本协议相关条款所做的修改，您有权停止使用本网站服务。如果您继续使用本网站服务，则视为您接受计全支付对本协议相关条款所做的修改。</p>', 'editor', 0, '2023-02-11 18:30:00');

#####  ----------  系统配置修改-支持代理商、商户系统配置  ----------  #####

ALTER TABLE `t_sys_config`   
  ADD COLUMN `sys_type` VARCHAR(8) NOT NULL COMMENT '所属系统: MGR-运营平台, AGENT-代理商平台, MCH-商户中心' AFTER `type`,
  ADD COLUMN `belong_info_id` VARCHAR(64) NOT NULL COMMENT '所属商户ID / 0(平台)' AFTER `sys_type`;

-- SELECT GROUP_CONCAT(CONCAT('''',group_key,'''')) FROM `t_sys_config`;
UPDATE `t_sys_config` SET `sys_type` = 'MGR', belong_info_id = 0 WHERE `group_key` IN ('agentTreatyConfig','agentTreatyConfig','applicationConfig','ossConfig','apiMapConfig','apiMapConfig','apiMapConfig','mchTreatyConfig','mchTreatyConfig','applicationConfig','applicationConfig','ossConfig','ossConfig','applicationConfig')

ALTER TABLE `t_sys_config`   
  DROP PRIMARY KEY,
  ADD PRIMARY KEY (`config_key`, `sys_type`, `belong_info_id`);
  
INSERT INTO `t_sys_config` VALUES ('appVoice', '是否启用app订单语音播报', '是否启用app订单语音播报', 'orderConfig', '系统配置', '1', 'radio', 'MCH', 'M0000000000', 0, '2023-03-23 23:30:00');
INSERT INTO `t_sys_config` VALUES ('qrcEscaping', '是否启用码牌防逃单功能', '是否启用码牌防逃单功能', 'orderConfig', '系统配置', '1', 'radio', 'MCH', 'M0000000000', 0, '2023-03-23 23:30:00');

INSERT INTO `t_sys_config` VALUES ('mchPayNotifyUrl', 'POS支付回调地址', 'POS支付回调地址', 'payOrderNotifyConfig', '回调和查单参数', '', 'text', 'MCH', 'M0000000000', 10, '2023-03-24 23:30:00');
INSERT INTO `t_sys_config` VALUES ('mchRefundNotifyUrl', 'POS退款回调地址', 'POS退款回调地址', 'payOrderNotifyConfig', '回调和查单参数', '', 'text', 'MCH', 'M0000000000', 20, '2023-03-24 23:30:00');
INSERT INTO `t_sys_config` VALUES ('mchNotifyPostType', '商户接收通知方式', '商户接收通知方式', 'payOrderNotifyConfig', '回调和查单参数', 'POST_JSON', 'radio', 'MCH', 'M0000000000', 30, '2023-03-24 23:30:00');
INSERT INTO `t_sys_config` VALUES ('payOrderNotifyExtParams', '支付订单回调和查单参数', '支付订单回调和查单参数', 'payOrderNotifyConfig', '回调和查单参数', '[]', 'text', 'MCH', 'M0000000000', 40, '2023-03-24 23:30:00');

INSERT INTO `t_sys_config` VALUES ('divisionConfig', '分账管理', '分账管理', 'divisionManage', '分账管理', '{"overrideAutoFlag":0,"autoDivisionRules":{"amountLimit":0,"delayTime":120},"mchDivisionEntFlag":1}', 'text', 'MCH', 'M0000000000', 0, '2023-03-25 13:30:00');

INSERT INTO `t_sys_config` VALUES ('mchApiEntList', '商户接口权限集合', '商户接口权限集合', 'mchApiEnt', '商户接口权限集合', '[]', 'text', 'MCH', 'M0000000000', 0, '2023-03-25 22:30:00');

#####  ----------  新增资源权限  ----------  #####

INSERT INTO t_sys_entitlement VALUES('ENT_C_MAIN_ISV_MCH_COUNT', '服务商/商户统计', 'no-icon', '', '', 'PB', 0, 1,  'ENT_C_MAIN', '0', 'MGR', NOW(), NOW());
INSERT INTO t_sys_entitlement VALUES('ENT_C_MAIN_PAY_DAY_COUNT', '今日/昨日交易统计', 'no-icon', '', '', 'PB', 0, 1,  'ENT_C_MAIN', '0', 'MGR', NOW(), NOW());
INSERT INTO t_sys_entitlement VALUES('ENT_C_MAIN_PAY_TREND_COUNT', '趋势图统计	', 'no-icon', '', '', 'PB', 0, 1,  'ENT_C_MAIN', '0', 'MGR', NOW(), NOW());

INSERT INTO t_sys_entitlement VALUES('ENT_C_MAIN_ISV_MCH_COUNT', '服务商/商户统计', 'no-icon', '', '', 'PB', 0, 1,  'ENT_C_MAIN', '0', 'AGENT', NOW(), NOW());
INSERT INTO t_sys_entitlement VALUES('ENT_C_MAIN_PAY_DAY_COUNT', '今日/昨日交易统计', 'no-icon', '', '', 'PB', 0, 1,  'ENT_C_MAIN', '0', 'AGENT', NOW(), NOW());
INSERT INTO t_sys_entitlement VALUES('ENT_C_MAIN_PAY_TREND_COUNT', '趋势图统计	', 'no-icon', '', '', 'PB', 0, 1,  'ENT_C_MAIN', '0', 'AGENT', NOW(), NOW());

INSERT INTO t_sys_entitlement VALUES('ENT_C_MAIN_PAY_COUNT', '主页交易统计', 'no-icon', '', '', 'PB', 0, 1,  'ENT_C_MAIN', '0', 'MCH', NOW(), NOW());
INSERT INTO t_sys_entitlement VALUES('ENT_C_MAIN_PAY_TYPE_COUNT', '主页交易方式统计', 'no-icon', '', '', 'PB', 0, 1,  'ENT_C_MAIN', '0', 'MCH', NOW(), NOW());
INSERT INTO t_sys_entitlement VALUES('ENT_C_MAIN_PAY_DAY_COUNT', '今日/昨日交易统计', 'no-icon', '', '', 'PB', 0, 1,  'ENT_C_MAIN', '0', 'MCH', NOW(), NOW());
INSERT INTO t_sys_entitlement VALUES('ENT_C_MAIN_PAY_TREND_COUNT', '趋势图统计	', 'no-icon', '', '', 'PB', 0, 1,  'ENT_C_MAIN', '0', 'MCH', NOW(), NOW());

INSERT INTO t_sys_entitlement VALUES('ENT_MCH_CONFIG_PAGE', '按钮：商户配置信息', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_INFO', '0', 'MGR', NOW(), NOW());

INSERT INTO t_sys_entitlement VALUES('ENT_MCH_OAUTH2_CONFIG_VIEW', '按钮：oauth2配置详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_INFO', '0', 'MGR', NOW(), NOW());
INSERT INTO t_sys_entitlement VALUES('ENT_MCH_OAUTH2_CONFIG_ADD', '按钮：oauth2配置', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_INFO', '0', 'MGR', NOW(), NOW());

INSERT INTO t_sys_entitlement VALUES('ENT_ISV_OAUTH2_CONFIG_VIEW', '按钮：oauth2配置详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_ISV_INFO', '0', 'MGR', NOW(), NOW());
INSERT INTO t_sys_entitlement VALUES('ENT_ISV_OAUTH2_CONFIG_ADD', '按钮：oauth2配置', 'no-icon', '', '', 'PB', 0, 1,  'ENT_ISV_INFO', '0', 'MGR', NOW(), NOW());

INSERT INTO t_sys_entitlement VALUES('ENT_MCH_INFO', '商户信息', 'user', '/info', 'MchInfoPage', 'ML', 0, 1,  'ENT_MCH_CENTER', '0', 'MCH', NOW(), NOW());

INSERT INTO t_sys_entitlement VALUES('ENT_MCH_CONFIG', '系统配置', 'setting', '/config', 'MchConfigPage', 'ML', 0, 1,  'ENT_SYS_CONFIG', '30', 'MCH', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_MCH_CONFIG_EDIT', '按钮：修改系统配置', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_CONFIG', '0', 'MCH', NOW(), NOW());
    
INSERT INTO t_sys_entitlement VALUES('ENT_DIVISION_RECORD_RESEND', '按钮：重试', 'no-icon', '', '', 'PB', 0, 1,  'ENT_DIVISION_RECORD', '0', 'MCH', NOW(), NOW());

INSERT INTO t_sys_entitlement VALUES('ENT_MCH_OAUTH2_CONFIG_VIEW', '按钮：oauth2配置详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_APP', '0', 'MCH', NOW(), NOW());
INSERT INTO t_sys_entitlement VALUES('ENT_MCH_OAUTH2_CONFIG_ADD', '按钮：oauth2配置', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_APP', '0', 'MCH', NOW(), NOW());

#####  ----------  修改表结构  ----------  #####

ALTER TABLE `t_mch_info`   
  ADD COLUMN `mch_level` VARCHAR(8) DEFAULT 'M0' NOT NULL COMMENT '商户级别: M0商户-简单模式（页面简洁，仅基础收款功能）, M1商户-高级模式（支持api调用，支持配置应用及分账、转账功能）' AFTER `type`,
  ADD COLUMN `refund_mode` JSON NULL COMMENT '退款方式[\"plat\", \"api\"],平台退款、接口退款，平台退款方式必须包含接口退款。' AFTER `mch_level`,
  ADD COLUMN `agent_no` VARCHAR(64) NULL COMMENT '代理商号' AFTER `refund_mode`;

ALTER TABLE `t_mch_info`   
  ADD COLUMN `sipw` VARCHAR(128) NOT NULL COMMENT '支付密码' AFTER `refund_mode`;

ALTER TABLE `t_mch_app`   
  ADD COLUMN `default_flag` TINYINT(6) DEFAULT 0 NOT NULL COMMENT '是否默认: 0-否, 1-是' AFTER `state`,
  ADD COLUMN `app_sign_type` JSON NOT NULL COMMENT '支持的签名方式 [\"MD5\", \"RSA2\"]' AFTER `default_flag`,
  CHANGE `app_secret` `app_secret` VARCHAR(128) NOT NULL COMMENT '应用MD5私钥',
  ADD COLUMN `app_rsa2_public_key` VARCHAR(448) NULL COMMENT 'RSA2应用公钥' AFTER `app_secret`;
  
-- select * from `t_mch_app` WHERE JSON_TYPE(app_sign_type) = 'NULL'
UPDATE `t_mch_app` SET `app_sign_type` = '["MD5"]' WHERE JSON_TYPE(app_sign_type) = 'NULL'

ALTER TABLE `t_pay_interface_define`
  ADD COLUMN `is_support_applyment` TINYINT(6) DEFAULT 0 NOT NULL COMMENT '是否支持进件: 0-不支持, 1-支持' AFTER `config_page_type`,
  ADD COLUMN `is_open_applyment` TINYINT(6) DEFAULT 0 NOT NULL COMMENT '是否开启进件: 0-关闭, 1-开启' AFTER `is_support_applyment`,
  ADD COLUMN `is_support_check_bill` TINYINT(6) DEFAULT 0 NOT NULL COMMENT '是否支持对账: 0-不支持, 1-支持' AFTER `is_open_applyment`,
  ADD COLUMN `is_open_check_bill` TINYINT(6) DEFAULT 0 NOT NULL COMMENT '是否开启对账: 0-关闭, 1-开启' AFTER `is_support_check_bill`,
  ADD COLUMN `is_support_cashout` TINYINT(6) DEFAULT 0 NOT NULL COMMENT '是否支持提现: 0-不支持, 1-支持' AFTER `is_open_check_bill`,
  ADD COLUMN `is_open_cashout` TINYINT(6) DEFAULT 0 NOT NULL COMMENT '是否开启提现: 0-关闭, 1-开启' AFTER `is_support_cashout`;

ALTER TABLE `t_pay_interface_config`
  CHANGE `info_type` `info_type` VARCHAR(20) NOT NULL COMMENT '账号类型:ISV-服务商, ISV_OAUTH2-服务商oauth2, AGENT-代理商, MCH_APP-商户应用, MCH_APP_OAUTH2-商户应用oauth2',
--   ADD COLUMN `config_mode` VARCHAR(20) NOT NULL COMMENT '配置模式: mgrIsv-服务商, mgrAgent-代理商, mgrMch-商户, agentSubagent-子代理商, agentMch-代理商商户, mchSelfApp1-小程序支付配置, mchSelfApp2-支付配置' AFTER `info_id`,
  ADD COLUMN `sett_hold_day` TINYINT DEFAULT 0 NOT NULL COMMENT '结算周期（自然日）' AFTER `if_params`
  ADD COLUMN `is_open_applyment` TINYINT(6) DEFAULT 0 NOT NULL COMMENT '是否开启进件: 0-关闭, 1-开启' AFTER `if_rate`,
  ADD COLUMN `is_open_cashout` TINYINT(6) DEFAULT 0 NOT NULL COMMENT '是否开启提现: 0-关闭, 1-开启' AFTER `is_open_applyment`,
  ADD COLUMN `is_open_check_bill` TINYINT(6) DEFAULT 0 NOT NULL COMMENT '是否开启对账: 0-关闭, 1-开启' AFTER `is_open_cashout`,
  ADD COLUMN `ignore_check_bill_mch_nos` VARCHAR(4096) NULL COMMENT '对账过滤子商户' AFTER `is_open_check_bill`;
  ;

-- ALTER TABLE `t_pay_way`   
--   ADD COLUMN `way_type` VARCHAR(20) NOT NULL COMMENT '支付类型: WECHAT-微信, ALIPAY-支付宝, YSFPAY-云闪付, UNIONPAY-银联, OTHER-其他' AFTER `way_name`;

-- SELECT * FROM `t_pay_way` WHERE `way_code` LIKE 'WX_%';
-- SELECT * FROM `t_pay_way` WHERE `way_code` LIKE 'ALI_%';
-- SELECT * FROM `t_pay_way` WHERE `way_code` LIKE 'YSF_%';
-- SELECT * FROM `t_pay_way` WHERE `way_code` LIKE 'PP_%';
-- 
-- UPDATE `t_pay_way` SET `way_type` = 'WECHAT' WHERE `way_code` LIKE 'WX_%';
-- UPDATE `t_pay_way` SET `way_type` = 'ALIPAY' WHERE `way_code` LIKE 'ALI_%';
-- UPDATE `t_pay_way` SET `way_type` = 'YSFPAY' WHERE `way_code` LIKE 'YSF_%';
-- UPDATE `t_pay_way` SET `way_type` = 'UNIONPAY' WHERE `way_code` LIKE 'UP_%';
-- UPDATE `t_pay_way` SET `way_type` = 'OTHER' WHERE `way_code` LIKE 'PP_%';

ALTER TABLE `t_pay_order`   
  ADD COLUMN `agent_no` VARCHAR(64) NULL COMMENT '代理商号' AFTER `mch_no`,
  ADD COLUMN `isv_name` VARCHAR(64) NULL COMMENT '服务商名称' AFTER `isv_no`,
  ADD COLUMN `isv_short_name` VARCHAR(32) NULL COMMENT '服务商简称' AFTER `isv_name`,
  ADD COLUMN `qrc_id` VARCHAR(64) NULL COMMENT '二维码' AFTER `app_id`,
  ADD COLUMN `app_name` VARCHAR(64) NULL COMMENT '应用名称' AFTER `app_id`,
  ADD COLUMN `store_id` VARCHAR(64) NULL COMMENT '门店ID' AFTER `app_name`,
  ADD COLUMN `store_name` VARCHAR(64) NULL COMMENT '门店名称' AFTER `store_id`,
  ADD COLUMN `mch_short_name` VARCHAR(32) NULL COMMENT '商户简称' AFTER `mch_name`,
  ADD COLUMN `seller_remark` VARCHAR(256) NULL COMMENT '买家备注' AFTER `body`,
  ADD COLUMN `buyer_remark` VARCHAR(256) NULL COMMENT '卖家备注' AFTER `seller_remark`;

ALTER TABLE `t_pay_order`   
  CHANGE `mch_name` `mch_name` VARCHAR(64) NOT NULL COMMENT '商户名称'  AFTER `mch_no`,
  CHANGE `mch_short_name` `mch_short_name` VARCHAR(32) NULL COMMENT '商户简称'  AFTER `mch_name`,
  ADD COLUMN `agent_name` VARCHAR(64) NULL COMMENT '代理商名称' AFTER `agent_no`,
  ADD COLUMN `agent_short_name` VARCHAR(32) NULL COMMENT '代理商简称' AFTER `agent_name`;

ALTER TABLE `t_pay_order`   
  CHANGE `mch_fee_amount` `mch_fee_amount` BIGINT(20) NOT NULL COMMENT '商户手续费(实际手续费),单位分',
  ADD COLUMN `mch_order_fee_amount` BIGINT(20) NULL COMMENT '收单手续费,单位分' AFTER `mch_fee_amount`,
  ADD COLUMN `channel_mch_no` VARCHAR(64) NULL COMMENT '渠道商户号' AFTER `buyer_remark`,
  ADD COLUMN `channel_isv_no` VARCHAR(64) NULL COMMENT '渠道服务商机构号' AFTER `channel_mch_no`,
  ADD COLUMN `platform_order_no` VARCHAR(64) NULL COMMENT '用户支付凭证交易单号 微信/支付宝流水号' AFTER `channel_order_no`,
  ADD COLUMN `platform_mch_order_no` VARCHAR(64) NULL COMMENT '用户支付凭证商户单号' AFTER `platform_order_no`;

ALTER TABLE `t_refund_order`   
  CHANGE `mch_name` `mch_name` VARCHAR(64) NOT NULL COMMENT '商户名称'  AFTER `mch_no`,
  ADD COLUMN `mch_short_name` VARCHAR(32) NULL COMMENT '商户简称' AFTER `mch_name`,
  ADD COLUMN `agent_no` VARCHAR(64) NULL COMMENT '代理商号' AFTER `mch_short_name`,
  ADD COLUMN `agent_name` VARCHAR(64) NULL COMMENT '代理商名称' AFTER `agent_no`,
  ADD COLUMN `agent_short_name` VARCHAR(32) NULL COMMENT '代理商简称' AFTER `agent_name`,
  CHANGE `isv_no` `isv_no` VARCHAR(64) NULL COMMENT '服务商号'  AFTER `agent_short_name`,
  ADD COLUMN `isv_name` VARCHAR(64) NULL COMMENT '服务商名称' AFTER `isv_no`,
  ADD COLUMN `isv_short_name` VARCHAR(32) NULL COMMENT '服务商简称' AFTER `isv_name`,
  CHANGE `app_id` `app_id` VARCHAR(64) NOT NULL COMMENT '应用ID'  AFTER `isv_short_name`,
  ADD COLUMN `app_name` VARCHAR(64) NULL COMMENT '应用名称' AFTER `app_id`,
  ADD COLUMN `store_id` VARCHAR(64) NULL COMMENT '门店ID' AFTER `app_name`,
  ADD COLUMN `store_name` VARCHAR(64) NULL COMMENT '门店名称' AFTER `store_id`;

ALTER TABLE `t_mch_notify_record`   
  ADD COLUMN `req_method` VARCHAR(10) NOT NULL COMMENT '通知请求方法' AFTER `notify_url`,
  ADD COLUMN `req_media_type` VARCHAR(10) NOT NULL COMMENT '通知请求媒体类型' AFTER `req_method`
  ADD COLUMN `req_body` TEXT NULL COMMENT '通知请求正文' AFTER `req_media_type`;
  
#####  ----------  代理商-表结构DDL+初始化DML  ----------  #####

-- 代理商信息表
DROP TABLE IF EXISTS `t_agent_info`;
CREATE TABLE `t_agent_info` (
  `agent_no` VARCHAR(64) NOT NULL COMMENT '代理商号',
  `agent_name` VARCHAR(64) NOT NULL COMMENT '代理商名称',
  `agent_short_name` VARCHAR(32) NOT NULL COMMENT '代理商简称',
  `agent_type` TINYINT NOT NULL DEFAULT '1' COMMENT '代理商类型: 1-个人, 2-企业',
  `level` TINYINT NOT NULL DEFAULT '1' COMMENT '等级: 1-一级, 2-二级, 3-三级 ...',
  `pid` VARCHAR(64) COMMENT '上级代理商号', 
  `isv_no` VARCHAR(64) DEFAULT NULL COMMENT '服务商号',
  `contact_name` VARCHAR(32) NOT NULL COMMENT '联系人姓名',
  `contact_tel` VARCHAR(32) NOT NULL COMMENT '联系人手机号',
  `contact_email` VARCHAR(32) NULL COMMENT '联系人邮箱',
  `add_agent_flag` TINYINT NOT NULL DEFAULT '0' COMMENT '是否允许发展下级: 0-否, 1-是',
  `state` TINYINT NOT NULL DEFAULT '1' COMMENT '状态: 0-停用, 1-正常',
  `sipw` VARCHAR(128) NOT NULL COMMENT '支付密码',
  `remark` VARCHAR(128) DEFAULT NULL COMMENT '备注',
  `init_user_id` BIGINT DEFAULT NULL COMMENT '初始用户ID（创建商户时，允许商户登录的用户）',
--   `login_user_name` VARCHAR(32) COMMENT '登录名', 
  `sett_account_type` VARCHAR(32) COMMENT '账户类型: ALIPAY_CASH-个人支付宝, WX_CASH-个人微信, BANK_PRIVATE-对私账户, BANK_PUBLIC-对公账户, BANK_CARD-银行卡',
  `sett_account_telphone` VARCHAR(32) COMMENT '结算人手机号', 
  `sett_account_name` VARCHAR(32) COMMENT '结算账户名', 
  `sett_account_no` VARCHAR(32) COMMENT '结算账号', 
  `sett_account_bank` VARCHAR(32) COMMENT '开户行名称', 
  `sett_account_sub_bank` VARCHAR(32) COMMENT '开户行支行名称', 
  `cashout_fee_rule_type` TINYINT NOT NULL DEFAULT '1' COMMENT '提现配置类型: 1-使用系统默认, 2-自定义', 
  `cashout_fee_rule` VARCHAR(255) DEFAULT NULL COMMENT '提现手续费规则', 
  `license_img` VARCHAR(128) DEFAULT NULL COMMENT '营业执照照片',
  `permit_img` VARCHAR(128) DEFAULT NULL COMMENT '开户许可证照片',
  `idcard1_img` VARCHAR(128) DEFAULT NULL COMMENT '身份证人像面照片',
  `idcard2_img` VARCHAR(128) DEFAULT NULL COMMENT '身份证国徽面照片',
  `idcard_in_hand_img` VARCHAR(128) DEFAULT NULL COMMENT '手持身份证照片',
  `bank_card_img` VARCHAR(128) DEFAULT NULL COMMENT '银行卡照片',
  `un_amount` INT NOT NULL DEFAULT '0' COMMENT '不可用金额', 
  `balance_amount` INT NOT NULL DEFAULT '0' COMMENT '钱包余额', 
  `audit_profit_amount` INT NOT NULL DEFAULT '0' COMMENT '在途佣金', 
  `created_uid` BIGINT DEFAULT NULL COMMENT '创建者用户ID',
  `created_by` VARCHAR(64) DEFAULT NULL COMMENT '创建者姓名',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (agent_no)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='代理商信息表';

-- 代理商管理
INSERT INTO t_sys_entitlement VALUES ('ENT_AGENT', '代理商管理', 'shop', '', 'RouteView', 'ML', 0, 1,  'ROOT', '35', 'MGR', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES ('ENT_AGENT_INFO', '代理商列表', 'profile', '/agent', 'AgentListPage', 'ML', 0, 1,  'ENT_AGENT', '10', 'MGR', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_AGENT_LIST', '页面：代理商列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_AGENT_INFO', '0', 'MGR', NOW(), NOW());
 	INSERT INTO t_sys_entitlement VALUES ('ENT_AGENT_PAY_CONFIG_LIST', '代理商支付参数配置列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_INFO', 0, 'MGR', NOW(), NOW());
 	INSERT INTO t_sys_entitlement VALUES ('ENT_AGENT_PAY_CONFIG_VIEW', '代理商支付参数配置详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_PAY_CONFIG_LIST', 0, 'MGR', NOW(), NOW());
 	INSERT INTO t_sys_entitlement VALUES ('ENT_AGENT_PAY_CONFIG_ADD', '代理商支付参数配置', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_PAY_CONFIG_LIST', 0, 'MGR', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_AGENT_INFO_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1,  'ENT_AGENT_INFO', '0', 'MGR', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_AGENT_INFO_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1,  'ENT_AGENT_INFO', '0', 'MGR', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_AGENT_INFO_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_AGENT_INFO', '0', 'MGR', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_AGENT_INFO_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1,  'ENT_AGENT_INFO', '0', 'MGR', NOW(), NOW());

#####  ↓↓↓↓↓↓↓↓↓↓  代理商管理初始化DML  ↓↓↓↓↓↓↓↓↓↓  #####

-- 权限表数据 （ 不包含根目录 ）
INSERT INTO t_sys_entitlement VALUES ('ENT_COMMONS', '系统通用菜单', 'no-icon', '', 'RouteView', 'MO', 0, 1, 'ROOT', -1, 'AGENT', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES ('ENT_C_USERINFO', '个人中心', 'no-icon', '/current/userinfo', 'CurrentUserInfo', 'MO', 0, 1, 'ENT_COMMONS', -1, 'AGENT', NOW(), NOW());

INSERT INTO t_sys_entitlement VALUES ('ENT_C_MAIN', '主页', 'home', '/main', 'MainPage', 'ML', 0, 1, 'ROOT', 1, 'AGENT', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES ('ENT_C_MAIN_PAY_TYPE_COUNT', '主页交易方式统计', 'no-icon', '', '', 'PB', 0, 1, 'ENT_C_MAIN', 0, 'AGENT', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES ('ENT_C_MAIN_PAY_COUNT', '主页交易统计', 'no-icon', '', '', 'PB', 0, 1, 'ENT_C_MAIN', 0, 'AGENT', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES ('ENT_C_MAIN_PAY_AMOUNT_WEEK', '主页周支付统计', 'no-icon', '', '', 'PB', 0, 1, 'ENT_C_MAIN', 0, 'AGENT', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES ('ENT_C_MAIN_NUMBER_COUNT', '主页数量总统计', 'no-icon', '', '', 'PB', 0, 1, 'ENT_C_MAIN', 0, 'AGENT', NOW(), NOW());

-- 商户管理
INSERT INTO t_sys_entitlement VALUES ('ENT_MCH', '商户管理', 'shop', '', 'RouteView', 'ML', 0, 1, 'ROOT', 30, 'AGENT', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_INFO', '商户列表', 'profile', '/mch', 'MchListPage', 'ML', 0, 1, 'ENT_MCH', 10, 'AGENT', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_LIST', '页面：商户列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_INFO', 0, 'AGENT', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_INFO_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_INFO', 0, 'AGENT', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_INFO_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_INFO', 0, 'AGENT', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_INFO_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_INFO', 0, 'AGENT', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_INFO_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_INFO', 0, 'AGENT', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_APP_CONFIG', '应用配置', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_INFO', 0, 'AGENT', NOW(), NOW());

    -- 应用管理
    INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_APP', '应用列表', 'appstore', '/apps', 'MchAppPage', 'ML', 0, 1, 'ENT_MCH', 20, 'AGENT', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_APP_LIST', '页面：应用列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_APP', 0, 'AGENT', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_APP_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_APP', 0, 'AGENT', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_APP_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_APP', 0, 'AGENT', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_APP_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_APP', 0, 'AGENT', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_APP_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_APP', 0, 'AGENT', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_PAY_PASSAGE_LIST', '应用支付通道配置列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_APP', 0, 'AGENT', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_PAY_PASSAGE_CONFIG', '应用支付通道配置入口', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_PAY_PASSAGE_LIST', 0, 'AGENT', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_PAY_PASSAGE_ADD', '应用支付通道配置保存', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_PAY_PASSAGE_LIST', 0, 'AGENT', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_PAY_CONFIG_VIEW', '应用支付参数配置详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_PAY_CONFIG_LIST', 0, 'AGENT', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_PAY_CONFIG_LIST', '应用支付参数配置列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_APP', 0, 'AGENT', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_PAY_CONFIG_ADD', '应用支付参数配置', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_PAY_CONFIG_LIST', 0, 'AGENT', NOW(), NOW());

-- 代理商管理
INSERT INTO t_sys_entitlement VALUES ('ENT_AGENT', '代理商管理', 'shop', '', 'RouteView', 'ML', 0, 1, 'ROOT', 35, 'AGENT', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES ('ENT_AGENT_INFO', '代理商列表', 'profile', '/agent', 'AgentListPage', 'ML', 0, 1, 'ENT_AGENT', 10, 'AGENT', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_AGENT_LIST', '页面：代理商列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_INFO', 0, 'AGENT', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_AGENT_PAY_CONFIG_LIST', '代理商支付参数配置列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_INFO', 0, 'AGENT', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_AGENT_PAY_CONFIG_VIEW', '代理商支付参数配置详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_PAY_CONFIG_LIST', 0, 'AGENT', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_AGENT_PAY_CONFIG_ADD', '代理商支付参数配置', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_PAY_CONFIG_LIST', 0, 'AGENT', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_AGENT_INFO_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_INFO', 0, 'AGENT', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_AGENT_INFO_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_INFO', 0, 'AGENT', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_AGENT_INFO_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_INFO', 0, 'AGENT', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_AGENT_INFO_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_INFO', 0, 'AGENT', NOW(), NOW());

-- 账户中心
INSERT INTO t_sys_entitlement VALUES ('ENT_AGENT_ACCOUNT_CENTER', '账户中心', 'wallet', '', 'RouteView', 'ML', 0, 1, 'ROOT', 5, 'AGENT', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES ('ENT_AGENT_STATISTIC', '数据统计', 'fund-view', '/statistic', 'StatisticsPage', 'ML', 0, 1, 'ENT_AGENT_ACCOUNT_CENTER', 20, 'AGENT', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_AGENT_CURRENT_INFO', '代理商信息', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_STATISTIC', 0, 'AGENT', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_AGENT_ORDER_STATISTIC', '订单/商户统计', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_STATISTIC', 0, 'AGENT', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_AGENT_STATISTIC_COUNT', '代理商统计', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_STATISTIC', 0, 'AGENT', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_AGENT_HARDWARE_STATISTIC', '硬件统计', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_STATISTIC', 0, 'AGENT', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES ('ENT_AGENT_SELF_PAY_CONFIG', '费率配置', 'file-done', '/passageConfig', 'PayConfigPage', 'ML', 0, 1, 'ENT_AGENT_ACCOUNT_CENTER', 30, 'AGENT', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_AGENT_SELF_PAY_CONFIG_LIST', '费率配置列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_SELF_PAY_CONFIG', 0, 'AGENT', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_AGENT_SELF_PAY_CONFIG_ADD', '费率配置保存', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_SELF_PAY_CONFIG', 0, 'AGENT', NOW(), NOW());
		
-- -- 服务商管理
-- INSERT INTO t_sys_entitlement VALUES ('ENT_ISV', '服务商管理', 'block', '', 'RouteView', 'ML', 0, 1, 'ROOT', 40, 'AGENT', NOW(), NOW());
--    INSERT INTO t_sys_entitlement VALUES ('ENT_ISV_INFO', '服务商列表', 'profile', '/isv', 'IsvListPage', 'ML', 0, 1, 'ENT_ISV', 10, 'AGENT', NOW(), NOW());
--        INSERT INTO t_sys_entitlement VALUES ('ENT_ISV_PAY_CONFIG_VIEW', '服务商支付参数配置详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_ISV_PAY_CONFIG_LIST', 0, 'AGENT', NOW(), NOW());
--        INSERT INTO t_sys_entitlement VALUES ('ENT_ISV_PAY_CONFIG_LIST', '服务商支付参数配置列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_ISV_INFO', 0, 'AGENT', NOW(), NOW());
--        INSERT INTO t_sys_entitlement VALUES ('ENT_ISV_PAY_CONFIG_ADD', '服务商支付参数配置', 'no-icon', '', '', 'PB', 0, 1, 'ENT_ISV_PAY_CONFIG_LIST', 0, 'AGENT', NOW(), NOW());
--        INSERT INTO t_sys_entitlement VALUES ('ENT_ISV_LIST', '页面：服务商列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_ISV_INFO', 0, 'AGENT', NOW(), NOW());
--        INSERT INTO t_sys_entitlement VALUES ('ENT_ISV_INFO_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_ISV_INFO', 0, 'AGENT', NOW(), NOW());
--        INSERT INTO t_sys_entitlement VALUES ('ENT_ISV_INFO_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1, 'ENT_ISV_INFO', 0, 'AGENT', NOW(), NOW());
--        INSERT INTO t_sys_entitlement VALUES ('ENT_ISV_INFO_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1, 'ENT_ISV_INFO', 0, 'AGENT', NOW(), NOW());
--        INSERT INTO t_sys_entitlement VALUES ('ENT_ISV_INFO_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1, 'ENT_ISV_INFO', 0, 'AGENT', NOW(), NOW());

-- 订单管理
INSERT INTO t_sys_entitlement VALUES ('ENT_ORDER', '订单管理', 'transaction', '', 'RouteView', 'ML', 0, 1, 'ROOT', 50, 'AGENT', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES ('ENT_PAY_ORDER', '支付订单', 'account-book', '/pay', 'PayOrderListPage', 'ML', 0, 1, 'ENT_ORDER', 10, 'AGENT', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_ORDER_LIST', '页面：订单列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_PAY_ORDER', 0, 'AGENT', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_PAY_ORDER_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_PAY_ORDER', 0, 'AGENT', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_PAY_ORDER_REFUND', '按钮：订单退款', 'no-icon', '', '', 'PB', 0, 1, 'ENT_PAY_ORDER', 0, 'AGENT', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_PAY_ORDER_SEARCH_PAY_WAY', '筛选项：支付方式', 'no-icon', '', '', 'PB', 0, 1, 'ENT_PAY_ORDER', 0, 'AGENT', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES ('ENT_REFUND_ORDER', '退款订单', 'exception', '/refund', 'RefundOrderListPage', 'ML', 0, 1, 'ENT_ORDER', 20, 'AGENT', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_REFUND_ORDER_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_REFUND_ORDER', 0, 'AGENT', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_REFUND_LIST', '页面：退款订单列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_REFUND_ORDER', 0, 'AGENT', NOW(), NOW());
--    INSERT INTO t_sys_entitlement VALUES ('ENT_TRANSFER_ORDER', '转账订单', 'property-safety', '/transfer', 'TransferOrderListPage', 'ML', 0, 1, 'ENT_ORDER', 25, 'AGENT', NOW(), NOW());
--        INSERT INTO t_sys_entitlement VALUES ('ENT_TRANSFER_ORDER_LIST', '页面：转账订单列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_TRANSFER_ORDER', 0, 'AGENT', NOW(), NOW());
--        INSERT INTO t_sys_entitlement VALUES ('ENT_TRANSFER_ORDER_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_TRANSFER_ORDER', 0, 'AGENT', NOW(), NOW());
--    INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_NOTIFY', '商户通知', 'notification', '/notify', 'MchNotifyListPage', 'ML', 0, 1, 'ENT_ORDER', 30, 'AGENT', NOW(), NOW());
--        INSERT INTO t_sys_entitlement VALUES ('ENT_NOTIFY_LIST', '页面：商户通知列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_NOTIFY', 0, 'AGENT', NOW(), NOW());
--        INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_NOTIFY_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_NOTIFY', 0, 'AGENT', NOW(), NOW());
--        INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_NOTIFY_RESEND', '按钮：重发通知', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_NOTIFY', 0, 'AGENT', NOW(), NOW());

-- -- 支付配置菜单
-- INSERT INTO t_sys_entitlement VALUES ('ENT_PC', '支付配置', 'file-done', '', 'RouteView', 'ML', 0, 1, 'ROOT', 60, 'AGENT', NOW(), NOW());
--     INSERT INTO t_sys_entitlement VALUES ('ENT_PC_IF_DEFINE', '支付接口', 'interaction', '/ifdefines', 'IfDefinePage', 'ML', 0, 1, 'ENT_PC', 10, 'AGENT', NOW(), NOW());
--         INSERT INTO t_sys_entitlement VALUES ('ENT_PC_IF_DEFINE_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_PC_IF_DEFINE', 0, 'AGENT', NOW(), NOW());
--         INSERT INTO t_sys_entitlement VALUES ('ENT_PC_IF_DEFINE_SEARCH', '页面：搜索', 'no-icon', '', '', 'PB', 0, 1, 'ENT_PC_IF_DEFINE', 0, 'AGENT', NOW(), NOW());
--         INSERT INTO t_sys_entitlement VALUES ('ENT_PC_IF_DEFINE_LIST', '页面：支付接口定义列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_PC_IF_DEFINE', 0, 'AGENT', NOW(), NOW());
--         INSERT INTO t_sys_entitlement VALUES ('ENT_PC_IF_DEFINE_EDIT', '按钮：修改', 'no-icon', '', '', 'PB', 0, 1, 'ENT_PC_IF_DEFINE', 0, 'AGENT', NOW(), NOW());
--         INSERT INTO t_sys_entitlement VALUES ('ENT_PC_IF_DEFINE_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1, 'ENT_PC_IF_DEFINE', 0, 'AGENT', NOW(), NOW());
--         INSERT INTO t_sys_entitlement VALUES ('ENT_PC_IF_DEFINE_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1, 'ENT_PC_IF_DEFINE', 0, 'AGENT', NOW(), NOW());
--     INSERT INTO t_sys_entitlement VALUES ('ENT_PC_WAY', '支付方式', 'appstore', '/payways', 'PayWayPage', 'ML', 0, 1, 'ENT_PC', 20, 'AGENT', NOW(), NOW());
--         INSERT INTO t_sys_entitlement VALUES ('ENT_PC_WAY_LIST', '页面：支付方式列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_PC_WAY', 0, 'AGENT', NOW(), NOW());
--         INSERT INTO t_sys_entitlement VALUES ('ENT_PC_WAY_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_PC_WAY', 0, 'AGENT', NOW(), NOW());
--         INSERT INTO t_sys_entitlement VALUES ('ENT_PC_WAY_SEARCH', '页面：搜索', 'no-icon', '', '', 'PB', 0, 1, 'ENT_PC_WAY', 0, 'AGENT', NOW(), NOW());
--         INSERT INTO t_sys_entitlement VALUES ('ENT_PC_WAY_EDIT', '按钮：修改', 'no-icon', '', '', 'PB', 0, 1, 'ENT_PC_WAY', 0, 'AGENT', NOW(), NOW());
--         INSERT INTO t_sys_entitlement VALUES ('ENT_PC_WAY_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1, 'ENT_PC_WAY', 0, 'AGENT', NOW(), NOW());
--         INSERT INTO t_sys_entitlement VALUES ('ENT_PC_WAY_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1, 'ENT_PC_WAY', 0, 'AGENT', NOW(), NOW());

-- 系统管理
INSERT INTO t_sys_entitlement VALUES ('ENT_SYS_CONFIG', '系统管理', 'setting', '', 'RouteView', 'ML', 0, 1, 'ROOT', 200, 'AGENT', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES ('ENT_UR', '用户角色管理', 'team', '', 'RouteView', 'ML', 0, 1, 'ENT_SYS_CONFIG', 10, 'AGENT', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_UR_USER', '操作员管理', 'contacts', '/users', 'SysUserPage', 'ML', 0, 1, 'ENT_UR', 10, 'AGENT', NOW(), NOW());
            INSERT INTO t_sys_entitlement VALUES ('ENT_UR_USER_VIEW', '按钮： 详情', '', 'no-icon', '', 'PB', 0, 1, 'ENT_UR_USER', 0, 'AGENT', NOW(), NOW());
            INSERT INTO t_sys_entitlement VALUES ('ENT_UR_USER_UPD_ROLE', '按钮： 角色分配', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_USER', 0, 'AGENT', NOW(), NOW());
            INSERT INTO t_sys_entitlement VALUES ('ENT_UR_USER_SEARCH', '按钮：搜索', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_USER', 0, 'AGENT', NOW(), NOW());
            INSERT INTO t_sys_entitlement VALUES ('ENT_UR_USER_LIST', '页面：操作员列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_USER', 0, 'AGENT', NOW(), NOW());
            INSERT INTO t_sys_entitlement VALUES ('ENT_UR_USER_EDIT', '按钮： 修改基本信息', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_USER', 0, 'AGENT', NOW(), NOW());
            INSERT INTO t_sys_entitlement VALUES ('ENT_UR_USER_DELETE', '按钮： 删除操作员', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_USER', 0, 'AGENT', NOW(), NOW());
            INSERT INTO t_sys_entitlement VALUES ('ENT_UR_USER_ADD', '按钮：添加操作员', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_USER', 0, 'AGENT', NOW(), NOW());

        INSERT INTO t_sys_entitlement VALUES ('ENT_UR_ROLE', '角色管理', 'user', '/roles', 'RolePage', 'ML', 0, 1, 'ENT_UR', 20, 'AGENT', NOW(), NOW());
            INSERT INTO t_sys_entitlement VALUES ('ENT_UR_ROLE_EDIT', '按钮： 修改基本信息', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_ROLE', 0, 'AGENT', NOW(), NOW());
            INSERT INTO t_sys_entitlement VALUES ('ENT_UR_ROLE_DIST', '按钮： 分配权限', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_ROLE', 0, 'AGENT', NOW(), NOW());
            INSERT INTO t_sys_entitlement VALUES ('ENT_UR_ROLE_DEL', '按钮： 删除', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_ROLE', 0, 'AGENT', NOW(), NOW());
            INSERT INTO t_sys_entitlement VALUES ('ENT_UR_ROLE_ADD', '按钮：添加角色', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_ROLE', 0, 'AGENT', NOW(), NOW());		
            INSERT INTO t_sys_entitlement VALUES ('ENT_UR_ROLE_SEARCH', '页面：搜索', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_ROLE', 0, 'AGENT', NOW(), NOW());
            INSERT INTO t_sys_entitlement VALUES ('ENT_UR_ROLE_LIST', '页面：角色列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_ROLE', 0, 'AGENT', NOW(), NOW());

--         INSERT INTO t_sys_entitlement VALUES ('ENT_UR_ROLE_ENT', '权限管理', 'apartment', '/ents', 'EntPage', 'ML', 0, 1, 'ENT_UR', 30, 'AGENT', NOW(), NOW());
--             INSERT INTO t_sys_entitlement VALUES ('ENT_UR_ROLE_ENT_LIST', '页面： 权限列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_ROLE_ENT', 0, 'AGENT', NOW(), NOW());
--             INSERT INTO t_sys_entitlement VALUES ('ENT_UR_ROLE_ENT_EDIT', '按钮： 权限变更', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_ROLE_ENT', 0, 'AGENT', NOW(), NOW());

--     INSERT INTO t_sys_entitlement VALUES ('ENT_AGENT_CONFIG', '系统配置', 'setting', '/config', 'AgentConfigPage', 'ML', 0, 1, 'ENT_SYS_CONFIG', 15, 'AGENT', NOW(), NOW());
--         INSERT INTO t_sys_entitlement VALUES ('ENT_AGENT_CONFIG_EDIT', '按钮： 修改', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_CONFIG', 0, 'AGENT', NOW(), NOW());

--     INSERT INTO t_sys_entitlement VALUES ('ENT_SYS_LOG', '系统日志', 'file-text', '/log', 'SysLogPage', 'ML', 0, 1, 'ENT_SYS_CONFIG', 20, 'AGENT', NOW(), NOW());
--         INSERT INTO t_sys_entitlement VALUES ('ENT_LOG_LIST', '页面：系统日志列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_SYS_LOG', 0, 'AGENT', NOW(), NOW());
--         INSERT INTO t_sys_entitlement VALUES ('ENT_SYS_LOG_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_SYS_LOG', 0, 'AGENT', NOW(), NOW());
--         INSERT INTO t_sys_entitlement VALUES ('ENT_SYS_LOG_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1, 'ENT_SYS_LOG', 0, 'AGENT', NOW(), NOW());

#####  ----------  商户门店-表结构DDL+初始化DML  ----------  #####

-- 商户门店表
DROP TABLE IF EXISTS `t_mch_store`;
CREATE TABLE `t_mch_store` (
  `store_id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '门店ID',
  `store_name` VARCHAR(64) NOT NULL DEFAULT '' COMMENT '门店名称',
  `mch_no` VARCHAR(64) NOT NULL COMMENT '商户号',
  `agent_no` VARCHAR(64) NULL COMMENT '代理商号',
  `isv_no` VARCHAR(64) DEFAULT NULL COMMENT '服务商号',
  `contact_phone` VARCHAR(32) NOT NULL COMMENT '联系人电话',
  `store_logo` VARCHAR(128) NOT NULL COMMENT '门店LOGO',
  `store_outer_img` VARCHAR(128) NOT NULL COMMENT '门头照',
  `store_inner_img` VARCHAR(128) NOT NULL COMMENT '门店内景照',
  `remark` VARCHAR(128) DEFAULT NULL COMMENT '备注',
  `province_code` VARCHAR(32) NOT NULL COMMENT '省代码',
  `city_code` VARCHAR(32) NOT NULL COMMENT '市代码',
  `district_code` VARCHAR(32) NOT NULL COMMENT '区代码',
  `address` VARCHAR(128) NOT NULL COMMENT '详细地址',
  `lng` VARCHAR(32) NOT NULL COMMENT '经度',
  `lat` VARCHAR(32) NOT NULL COMMENT '纬度',
  `default_flag` TINYINT NOT NULL DEFAULT '0' COMMENT '是否默认: 0-否, 1-是',
  `bind_app_id` VARCHAR(64) NULL COMMENT '绑定AppId',
  `created_uid` BIGINT DEFAULT NULL COMMENT '创建者用户ID',
  `created_by` VARCHAR(64) DEFAULT NULL COMMENT '创建者姓名',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`store_id`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci AUTO_INCREMENT=1001 COMMENT='商户门店表';
  
-- 门店管理
INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE', '门店管理', 'profile', '/store', 'MchStorePage', 'ML', 0, 1,  'ENT_MCH', '40', 'MGR', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE_LIST', '页面：门店列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'MGR', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'MGR', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'MGR', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE_APP_DIS', '按钮：应用分配', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'MGR', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'MGR', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'MGR', NOW(), NOW());
    
-- 门店管理
INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE', '门店管理', 'profile', '/store', 'MchStorePage', 'ML', 0, 1,  'ENT_MCH', '40', 'AGENT', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE_LIST', '页面：门店列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'AGENT', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'AGENT', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'AGENT', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE_APP_DIS', '按钮：应用分配', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'AGENT', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'AGENT', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'AGENT', NOW(), NOW());
    
-- 门店管理
INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE', '门店管理', 'shop', '/store', 'MchStorePage', 'ML', 0, 1,  'ENT_MCH_CENTER', '60', 'MCH', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE_LIST', '页面：门店列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'MCH', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'MCH', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'MCH', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE_APP_DIS', '按钮：应用分配', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'MCH', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'MCH', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'MCH', NOW(), NOW());
   
#####  ----------  商户进件-表结构DDL+初始化DML  ----------  #####

-- 商户进件表
DROP TABLE IF EXISTS `t_mch_apply`;
CREATE TABLE `t_mch_apply` (
  `apply_id` VARCHAR(64) NOT NULL COMMENT '进件单号',
  `mch_no` VARCHAR(64) NOT NULL COMMENT '商户号',
  `agent_no` VARCHAR(64) NOT NULL COMMENT '代理商号',
  `isv_no` VARCHAR(64) DEFAULT NULL COMMENT '服务商号',
  `if_code` VARCHAR(20) NOT NULL COMMENT '接口代码 全小写  wxpay alipay ',
  `apply_page_type` VARCHAR(20) NOT NULL COMMENT '来源: PLATFORM_WEB-运营平台(WEB) AGENT_WEB-代理商(WEB) MCH_WEB-商户(WEB) AGENT_LITE-代理商(小程序) MCH_LITE-商户(小程序)',
  `apply_detail_info` VARCHAR(255) NOT NULL COMMENT '申请详细信息', 
  `apply_error_info` VARCHAR(255) NOT NULL COMMENT '申请错误信息(响应提示信息)', 
  `channel_apply_no` VARCHAR(64) NOT NULL COMMENT '渠道申请单号', 
  `succ_res_parameter` VARCHAR(255) NOT NULL COMMENT '成功响应参数(渠道响应参数)', 
  `channel_var1` VARCHAR(255) NOT NULL COMMENT '渠道拓展参数1', 
  `channel_var2` VARCHAR(255) NOT NULL COMMENT '渠道拓展参数2', 
  `state` TINYINT NOT NULL DEFAULT '0' COMMENT '状态: 0-草稿, 1-审核中, 2-进件成功, 3-驳回待修改, 4-待验证, 5-待签约, 7-等待预审, 8-预审拒绝', 
  `is_temp_data` BOOLEAN NOT NULL COMMENT '是否临时数据', 
  `mch_full_name` VARCHAR(64) NOT NULL COMMENT '商户名称全称', 
  `mch_short_name` VARCHAR(32) NOT NULL COMMENT '进件商户简称', 
  `merchant_type` TINYINT NOT NULL DEFAULT '0' COMMENT '商户类型: 1-个人, 2-个体工商户, 3-企业', 
  `contact_name` VARCHAR(32) NOT NULL COMMENT '商户联系人姓名', 
  `contact_phone` VARCHAR(32) NOT NULL COMMENT '商户联系人电话', 
  `contact_email` VARCHAR(32) NULL COMMENT '商户联系人邮箱', 
  `province_code` VARCHAR(32) NOT NULL COMMENT '省代码',
  `city_code` VARCHAR(32) NOT NULL COMMENT '市代码',
  `district_code` VARCHAR(32) NOT NULL COMMENT '区代码',
  `address` VARCHAR(128) NOT NULL COMMENT '商户详细地址',
  `ep_user_id` BIGINT DEFAULT NULL COMMENT '商户拓展员ID',
  `created_uid` BIGINT DEFAULT NULL COMMENT '创建者用户ID',
  `created_by` VARCHAR(64) DEFAULT NULL COMMENT '创建者姓名',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `last_apply_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '上次进件时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (apply_id)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='商户进件申请表';

#####  ----------  团队-表结构DDL+初始化DML  ----------  #####

-- 团队信息表
DROP TABLE IF EXISTS `t_sys_user_team`;
CREATE TABLE `t_sys_user_team` (
  `team_id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '团队ID',
  `team_name` VARCHAR(32) NOT NULL COMMENT '团队名称', 
  `team_no` VARCHAR(64) NOT NULL COMMENT '团队编号', 
  `sys_type` VARCHAR(8) NOT NULL COMMENT '所属系统: MGR-运营平台, AGENT-代理商平台, MCH-商户中心', 
  `stat_range_type` VARCHAR(20) NOT NULL COMMENT '统计周期: year-年, quarter-季度, month-月, week-周', 
  `belong_info_id` VARCHAR(64) NOT NULL COMMENT '归属信息ID', 
  `created_uid` BIGINT DEFAULT NULL COMMENT '创建者用户ID',
  `created_by` VARCHAR(64) DEFAULT NULL COMMENT '创建者姓名',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (team_id)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='团队信息表';
  
ALTER TABLE `t_sys_user`   
  CHANGE `sex` `sex` TINYINT DEFAULT 0 NOT NULL COMMENT '性别: 0-未知, 1-男, 2-女',
  CHANGE `is_admin` `is_admin` TINYINT DEFAULT 0 NOT NULL COMMENT '是否超管（超管拥有全部权限）: 0-否 1-是',
  CHANGE `sys_type` `sys_type` VARCHAR(8) NOT NULL COMMENT '所属系统: MGR-运营平台, AGENT-代理商平台, MCH-商户中心',
  ADD COLUMN `user_type` TINYINT(6) NOT NULL DEFAULT 1 COMMENT '用户类型: 1-超级管理员, 2-普通操作员, 3-商户拓展员, 11-店长, 12-店员' AFTER `sys_type`,
  ADD COLUMN `invite_code` VARCHAR(20) NULL COMMENT '邀请码' AFTER `user_type`,
  ADD COLUMN `team_id` BIGINT NULL COMMENT '团队ID' AFTER `invite_code`,
--   ADD COLUMN `team_name` VARCHAR(32) NULL COMMENT '团队名称' AFTER `team_id`,
  ADD COLUMN `is_team_leader` TINYINT NULL COMMENT '是否队长:  0-否 1-是' AFTER `team_id`;

ALTER TABLE `t_sys_user`   
  ADD UNIQUE INDEX `invite_code` (`invite_code`);
  
ALTER TABLE `t_sys_user`   
  ADD COLUMN `safe_word` VARCHAR(32) NULL COMMENT '预留信息' AFTER `realname`;
  
INSERT INTO t_sys_entitlement VALUES ('ENT_UR_TEAM', '团队管理', 'team', '/teams', 'SysUserTeamPage', 'ML', 0, 1, 'ENT_UR', 15, 'MGR', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES ('ENT_UR_TEAM_LIST', '页面：团队列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_TEAM', 0, 'MGR', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES ('ENT_UR_TEAM_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_TEAM', 0, 'MGR', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES ('ENT_UR_TEAM_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_TEAM', 0, 'MGR', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES ('ENT_UR_TEAM_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_TEAM', 0, 'MGR', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES ('ENT_UR_TEAM_DEL', '按钮： 删除', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_TEAM', 0, 'MGR', NOW(), NOW());
	
INSERT INTO t_sys_entitlement VALUES ('ENT_UR_TEAM', '团队管理', 'team', '/teams', 'SysUserTeamPage', 'ML', 0, 1, 'ENT_UR', 15, 'AGENT', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES ('ENT_UR_TEAM_LIST', '页面：团队列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_TEAM', 0, 'AGENT', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES ('ENT_UR_TEAM_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_TEAM', 0, 'AGENT', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES ('ENT_UR_TEAM_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_TEAM', 0, 'AGENT', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES ('ENT_UR_TEAM_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_TEAM', 0, 'AGENT', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES ('ENT_UR_TEAM_DEL', '按钮： 删除', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_TEAM', 0, 'AGENT', NOW(), NOW());

#####  ----------  文章-表结构DDL+初始化DML  ----------  #####

-- 文章信息表
CREATE TABLE `t_sys_article`(  
  `article_id` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT '文章ID',
  `title` VARCHAR(64) NOT NULL COMMENT '文章标题',
  `subtitle` VARCHAR(64) NOT NULL COMMENT '文章副标题',
  `article_type` TINYINT(6) NOT NULL DEFAULT 1 COMMENT '文章类型: 1-公告',
  `article_range` JSON NOT NULL COMMENT '文章范围',
  `content` TEXT COMMENT '文章内容',
  `publisher` VARCHAR(32) NOT NULL COMMENT '发布人',
  `publish_time` TIMESTAMP(6) COMMENT '发布时间',
  `created_uid` BIGINT(20) COMMENT '创建者用户ID',
  `created_by` VARCHAR(64) COMMENT '创建者姓名',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`article_id`)
) ENGINE=INNODB AUTO_INCREMENT=1001 CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='文章信息表';

-- 公告管理
INSERT INTO t_sys_entitlement VALUES('ENT_ARTICLE_NOTICEINFO', '公告管理', 'message', '/notices', 'NoticeInfoPage', 'ML', 0, 1,  'ENT_SYS_CONFIG', '30', 'MGR', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_NOTICE_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1,  'ENT_ARTICLE_NOTICEINFO', '0', 'MGR', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_NOTICE_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1,  'ENT_ARTICLE_NOTICEINFO', '0', 'MGR', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_NOTICE_EDIT', '按钮：修改', 'no-icon', '', '', 'PB', 0, 1,  'ENT_ARTICLE_NOTICEINFO', '0', 'MGR', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_NOTICE_LIST', '页面：列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_ARTICLE_NOTICEINFO', '0', 'MGR', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_NOTICE_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_ARTICLE_NOTICEINFO', '0', 'MGR', NOW(), NOW());

INSERT INTO t_sys_entitlement VALUES('ENT_ARTICLE_NOTICEINFO', '公告管理', 'message', '/notices', 'NoticeInfoPage', 'MO', 0, 1,  'ENT_COMMONS', '-1', 'AGENT', NOW(), NOW());
INSERT INTO t_sys_entitlement VALUES('ENT_ARTICLE_NOTICEINFO', '公告管理', 'message', '/notices', 'NoticeInfoPage', 'MO', 0, 1,  'ENT_COMMONS', '-1', 'MCH', NOW(), NOW());

#####  ----------  支付费率配置-表结构DDL+初始化DML  ----------  #####

-- 支付费率配置表
DROP TABLE IF EXISTS `t_pay_rate_config`;
CREATE TABLE `t_pay_rate_config` (
  `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT 'ID',
--   `config_mode` VARCHAR(20) NOT NULL COMMENT '配置模式: mgrIsv-服务商, mgrAgent-代理商, agentSubagent-子代理商, mgrMch-商户, agentMch-代理商商户',
  `config_type` VARCHAR(20) NOT NULL COMMENT '配置类型: ISVCOST-服务商低价, AGENTRATE-代理商费率, AGENTDEF-代理商默认费率, MCHAPPLYDEF-商户进件默认费率, MCHRATE-商户费率',
  `info_type` VARCHAR(20) NOT NULL COMMENT '账号类型:ISV-服务商, AGENT-代理商, MCH_APP-商户应用, MCH_APPLY-商户进件',
  `info_id` VARCHAR(64) NOT NULL COMMENT '服务商号/商户号/应用ID',
  `if_code` VARCHAR(20) NOT NULL COMMENT '支付接口',
  `way_code` VARCHAR(20) NOT NULL COMMENT '支付方式',
  `fee_type` VARCHAR(20) NOT NULL COMMENT '费率类型:SINGLE-单笔费率, LEVEL-阶梯费率',
  `level_mode` VARCHAR(20) NULL COMMENT '阶梯模式: 模式: NORMAL-普通模式, UNIONPAY-银联模式',
  `fee_rate` DECIMAL(20,6) NULL COMMENT '支付方式费率',
  `applyment_support` TINYINT NOT NULL COMMENT '是否支持进件: 0-不支持, 1-支持',
  `state` TINYINT NOT NULL COMMENT '状态: 0-停用, 1-启用',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`id`),
  UNIQUE KEY `Uni_InfoId_WayCode` (`config_type`,`info_type`,`info_id`,`if_code`,`way_code`)
) ENGINE=INNODB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='支付费率配置表';

-- 支付费率阶梯配置表
DROP TABLE IF EXISTS `t_pay_rate_level_config`;
CREATE TABLE `t_pay_rate_level_config` (
  `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT 'ID',
  `rate_config_id` BIGINT NOT NULL COMMENT '支付费率配置ID',
  `bank_card_type` VARCHAR(20) NULL COMMENT '银行卡类型: DEBIT-借记卡（储蓄卡）, CREDIT-贷记卡（信用卡）',
  `min_amount` INT NOT NULL DEFAULT '0' COMMENT '最小金额: 计算时大于此值', 
  `max_amount` INT NOT NULL DEFAULT '0' COMMENT '最大金额: 计算时小于或等于此值', 
  `min_fee` INT NOT NULL DEFAULT '0' COMMENT '保底费用', 
  `max_fee` INT NOT NULL DEFAULT '0' COMMENT '封顶费用', 
  `fee_rate` DECIMAL(20,6) NOT NULL COMMENT '支付方式费率',
  `state` TINYINT NOT NULL COMMENT '状态: 0-停用, 1-启用',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`id`)
) ENGINE=INNODB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='支付费率阶梯配置表';

#####  ----------  码牌-表结构DDL+初始化DML  ----------  #####

-- 码牌模板信息表
DROP TABLE IF EXISTS `t_qr_code_shell`;
CREATE TABLE `t_qr_code_shell` (
  `id` INT(11) NOT NULL AUTO_INCREMENT COMMENT '码牌模板ID',
  `style_code` VARCHAR(20) NOT NULL COMMENT '样式代码: shellA, shellB', 
  `shell_alias` VARCHAR(20) NOT NULL COMMENT '模板别名',
  `config_info` VARCHAR(4096) NOT NULL COMMENT '模板配置信息,json字符串',
  `shell_img_view_url` VARCHAR(255) COMMENT '模板预览图Url',
--   `state` TINYINT NOT NULL COMMENT '状态: 0-停用, 1-启用',
  `sys_type` VARCHAR(8) NOT NULL COMMENT '所属系统: MGR-运营平台, AGENT-代理商平台, MCH-商户中心', 
  `belong_info_id` VARCHAR(64) NOT NULL COMMENT '归属信息ID', 
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (id)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='码牌模板信息表';

-- 码牌信息表
DROP TABLE IF EXISTS `t_qr_code`;
CREATE TABLE `t_qr_code` (
  -- `id` INT(11) NOT NULL AUTO_INCREMENT,
  `qrc_id` VARCHAR(64) NOT NULL COMMENT '码牌ID',
  `qrc_shell_id` INT COMMENT '模板ID', 
  `batch_id` VARCHAR(64) NULL COMMENT '批次号',
  `fixed_flag` TINYINT NOT NULL COMMENT '是否固定金额: 0-任意金额, 1-固定金额', 
  `fixed_pay_amount` INT NOT NULL DEFAULT '0' COMMENT '固定金额',
  `entry_page` VARCHAR(20) NOT NULL COMMENT '选择页面类型: default-默认(未指定，取决于二维码是否绑定到微信侧), h5-固定H5页面, lite-固定小程序页面', 
  `alipay_way_code` VARCHAR(20) NOT NULL COMMENT '支付宝支付方式(仅H5呈现时生效): ALI_JSAPI ALI_WAP', 
  `qrc_alias` VARCHAR(20) COMMENT '码牌别名',
  `bind_state` TINYINT NOT NULL COMMENT '码牌绑定状态: 0-未绑定, 1-已绑定',  
  `agent_no` VARCHAR(64) NULL COMMENT '代理商号',
  `mch_no` VARCHAR(64) NULL COMMENT '商户号',
  -- `mch_name` VARCHAR(30) NOT NULL COMMENT '商户名称',
  `app_id` VARCHAR(64) NULL COMMENT '应用ID',
  `store_id` BIGINT NULL COMMENT '门店ID',
  `qr_url` VARCHAR(255) NOT NULL COMMENT '二维码Url', 
  -- `qrc_state` TINYINT NOT NULL COMMENT '状态: 0-停用, 1-启用', 
  `state` TINYINT NOT NULL COMMENT '状态: 0-停用, 1-启用',
  -- `qrc_belong_type` INT NOT NULL DEFAULT '1' COMMENT '获取方式: 1-自制, 2-下发',
  `sys_type` VARCHAR(8) NOT NULL COMMENT '所属系统: MGR-运营平台, AGENT-代理商平台, MCH-商户中心', 
  `belong_info_id` VARCHAR(64) NOT NULL COMMENT '归属信息ID', 
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (qrc_id)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='码牌信息表';

-- 设备配置
INSERT INTO t_sys_entitlement VALUES ('ENT_DEVICE', '设备配置', 'appstore', '', 'RouteView', 'ML', 0, 1,  'ROOT', '70', 'MGR', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES ('ENT_QRC', '码牌', 'shop', '', 'RouteView', 'ML', 0, 1,  'ENT_DEVICE', '10', 'MGR', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_DEVICE_QRC_SHELL', '模板管理', 'file', '/shell', 'QrCodeShellPage', 'ML', 0, 1,  'ENT_QRC', '10', 'MGR', NOW(), NOW());
            INSERT INTO t_sys_entitlement VALUES ('ENT_DEVICE_QRC_SHELL_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1,  'ENT_DEVICE_QRC_SHELL', '0', 'MGR', NOW(), NOW());
 	    INSERT INTO t_sys_entitlement VALUES ('ENT_DEVICE_QRC_SHELL_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1, 'ENT_DEVICE_QRC_SHELL', '0', 'MGR', NOW(), NOW());
 	    INSERT INTO t_sys_entitlement VALUES ('ENT_DEVICE_QRC_SHELL_EDIT', '按钮：修改', 'no-icon', '', '', 'PB', 0, 1, 'ENT_DEVICE_QRC_SHELL', '0', 'MGR', NOW(), NOW());
 	    INSERT INTO t_sys_entitlement VALUES ('ENT_DEVICE_QRC_SHELL_LIST', '页面：列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_DEVICE_QRC_SHELL', '0', 'MGR', NOW(), NOW());
 	    INSERT INTO t_sys_entitlement VALUES ('ENT_DEVICE_QRC_SHELL_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_DEVICE_QRC_SHELL', '0', 'MGR', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_DEVICE_QRC', '码牌管理', 'qrcode', '/qrc', 'QrCodePage', 'ML', 0, 1,  'ENT_QRC', '20', 'MGR', NOW(), NOW());
            INSERT INTO t_sys_entitlement VALUES ('ENT_DEVICE_QRC_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1,  'ENT_DEVICE_QRC', '0', 'MGR', NOW(), NOW());
 	    INSERT INTO t_sys_entitlement VALUES ('ENT_DEVICE_QRC_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1, 'ENT_DEVICE_QRC', '0', 'MGR', NOW(), NOW());
 	    INSERT INTO t_sys_entitlement VALUES ('ENT_DEVICE_QRC_EDIT', '按钮：修改', 'no-icon', '', '', 'PB', 0, 1, 'ENT_DEVICE_QRC', '0', 'MGR', NOW(), NOW());
 	    INSERT INTO t_sys_entitlement VALUES ('ENT_DEVICE_QRC_LIST', '页面：列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_DEVICE_QRC', '0', 'MGR', NOW(), NOW());
 	    INSERT INTO t_sys_entitlement VALUES ('ENT_DEVICE_QRC_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_DEVICE_QRC', '0', 'MGR', NOW(), NOW());
 	    INSERT INTO t_sys_entitlement VALUES ('ENT_DEVICE_QRC_EXPORT', '按钮：导出', 'no-icon', '', '', 'PB', 0, 1, 'ENT_DEVICE_QRC', '0', 'MGR', NOW(), NOW());

#####  ----------  供应商、设备-表结构DDL+初始化DML  ----------  #####

-- 供应商定义表
DROP TABLE IF EXISTS t_provider_define;
CREATE TABLE `t_provider_define` (
  `provider_code` VARCHAR(20) NOT NULL COMMENT '供应商代码 全小写 zgwl',
  `provider_name` VARCHAR(20) NOT NULL COMMENT '供应商名称',
--   `provider_type` TINYINT(6) NOT NULL COMMENT '供应商类型:1-云音响 2-云打印',
  `config_page_type` TINYINT(6) NOT NULL DEFAULT 1 COMMENT '支付参数配置页面类型:1-JSON渲染,2-自定义',
  `provider_params` VARCHAR(4096) DEFAULT NULL COMMENT '供应商配置定义描述,json字符串',
  `device_types` JSON NOT NULL COMMENT '支持设备类型 [1, 2]',
  `icon` VARCHAR(256) DEFAULT NULL COMMENT '页面展示：卡片-图标',
  `bg_color` VARCHAR(20) DEFAULT NULL COMMENT '页面展示：卡片-背景色',
  `state` TINYINT(6) NOT NULL DEFAULT 1 COMMENT '状态: 0-停用, 1-启用',
  `remark` VARCHAR(128) DEFAULT NULL COMMENT '备注',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`provider_code`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COMMENT='供应商定义表';

-- 设备配置参数表
DROP TABLE IF EXISTS t_device_config;
CREATE TABLE `t_device_config` (
  `id` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT 'ID',
  `device_type` TINYINT(6) NOT NULL COMMENT '设备类型:1-云音响 2-云打印',
  `provider_code` VARCHAR(20) NOT NULL COMMENT '供应商代码',
  `provider_params` VARCHAR(4096) NOT NULL COMMENT '接口配置参数,json字符串',
  `state` TINYINT(6) NOT NULL DEFAULT 1 COMMENT '状态: 0-停用, 1-启用',
  `remark` VARCHAR(128) DEFAULT NULL COMMENT '备注',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`id`),
  UNIQUE KEY `Uni_DeviceType_Provider` (`device_type`, `provider`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COMMENT='设备配置参数表';

INSERT INTO t_provider_define (provider_code, provider_name, config_page_type, provider_params, isvsub_mch_params, device_types, icon, bg_color, state, remark)
VALUES ('zgwl', '智谷联', 1,
        '[{"name":"accessKeyId","desc":"AccessKeyId","type":"text","verify":"required","star":"1"},{"name":"accessKeySecret","desc":"AccessKeySecret","type":"text","verify":"required","star":"1"},{"name":"instanceId","desc":"实例ID","type":"text","verify":"required"},{"name":"endPoint","desc":"公网接入点","type":"text","verify":"required"},{"name":"topic","desc":"Topic","type":"text","verify":"required"},{"name":"groupId","desc":"GroupId","type":"text","verify":"required"}]',
        '[1, 2]'
        'http://jeequan.oss-cn-beijing.aliyuncs.com/jeepay/img/alipay.png', '#1779FF', 1, '智谷联');

-- 设备信息表
DROP TABLE IF EXISTS t_device_info;
CREATE TABLE `t_device_config` (
  `id` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT 'ID',
  `device_no` VARCHAR(64) NOT NULL COMMENT '设备号',
  `device_name` VARCHAR(64) NOT NULL COMMENT '设备名称',
  `device_config_id` BIGINT(20) NOT NULL COMMENT '设备配置参数ID',
  `batch_id` VARCHAR(64) NULL COMMENT '批次号',
  `bind_state` TINYINT NOT NULL COMMENT '绑定状态: 0-未绑定, 1-已绑定',
  `bind_type` TINYINT NOT NULL COMMENT '绑定类型: 0-门店, 1-码牌', 
  `mch_no` VARCHAR(64) NOT NULL COMMENT '商户号',
  `app_id` VARCHAR(64) NOT NULL COMMENT '应用ID',
  `store_id` BIGINT NOT NULL COMMENT '门店ID',
  `bind_qrc_id` BIGINT NOT NULL COMMENT '绑定码牌ID',
  `state` TINYINT(6) NOT NULL DEFAULT 1 COMMENT '状态: 0-停用, 1-启用',
  `remark` VARCHAR(128) DEFAULT NULL COMMENT '备注',
  `sys_type` VARCHAR(8) NOT NULL COMMENT '所属系统: MGR-运营平台, AGENT-代理商平台, MCH-商户中心', 
  `belong_info_id` VARCHAR(64) NOT NULL COMMENT '归属信息ID', 
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`id`),
  UNIQUE KEY `Uni_DeviceNo` (`device_no`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COMMENT='设备信息表';


#####  ----------  商家会员-表结构DDL+初始化DML  ----------  #####

-- 商户会员信息表
DROP TABLE IF EXISTS t_mbr_info;
CREATE TABLE `t_mch_info` (
  `mbr_no` VARCHAR(64) NOT NULL COMMENT '会员号',
  `mbr_name` VARCHAR(64) NOT NULL COMMENT '会员名称',
  `mch_no` VARCHAR(64) NOT NULL COMMENT '商户号',
  `mbr_tel` VARCHAR(32) COMMENT '手机号',
  `balance` BIGINT(20) NOT NULL DEFAULT 0 COMMENT '账户余额,单位分',
  `ali_user_id` VARCHAR(64) COMMENT '支付宝用户ID',
  `wx_mp_open_id` VARCHAR(64) COMMENT '微信公众平台OpenId',
  `avatar_url` VARCHAR(128) DEFAULT NULL COMMENT '头像地址',
  `remark` VARCHAR(128) COMMENT '会员备注',
  `state` TINYINT(6) NOT NULL DEFAULT 1 COMMENT '会员状态: 0-停用, 1-正常',
  `created_uid` BIGINT(20) COMMENT '创建者用户ID',
  `created_by` VARCHAR(64) COMMENT '创建者姓名',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`mbr_no`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COMMENT='商户会员信息表';

-- 商户会员账户交易表
DROP TABLE IF EXISTS t_mbr_trade;
CREATE TABLE `t_mbr_trade` (
  `trade_id` VARCHAR(30) NOT NULL COMMENT '会员交易单号',
  `mbr_no` VARCHAR(64) NOT NULL COMMENT '会员号',
  `mbr_name` VARCHAR(64) NOT NULL COMMENT '会员名称',
  `mch_no` VARCHAR(64) NOT NULL COMMENT '商户号',
  `mbr_tel` VARCHAR(32) COMMENT '手机号',
  `before_amount` BIGINT(20) NOT NULL DEFAULT 0 COMMENT '变动前余额,单位分',
  `change_balance` BIGINT(20) NOT NULL DEFAULT 0 COMMENT '变动金额,单位分',
  `after_balance` BIGINT(20) NOT NULL DEFAULT 0 COMMENT '变动后余额,单位分',
  `biz_type` TINYINT(6) NOT NULL DEFAULT 1 COMMENT '业务类型: 0-, 1-支付充值, 2-现金充值, 3-会员消费, 4-消费退款, 5-人工调账',
  `remark` VARCHAR(128) COMMENT '交易备注',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`trade_id`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COMMENT='商户会员账户交易表';

-- 商户会员充值记录表
DROP TABLE IF EXISTS t_mbr_recharge_record;
CREATE TABLE `t_mbr_recharge_record` (
  `recharge_record_id` VARCHAR(30) NOT NULL COMMENT '充值记录ID',
  `mbr_no` VARCHAR(64) NOT NULL COMMENT '会员号',
  `mbr_name` VARCHAR(64) NOT NULL COMMENT '会员名称',
  `mch_no` VARCHAR(64) NOT NULL COMMENT '商户号',
  `mbr_tel` VARCHAR(32) COMMENT '手机号',
  `pay_order_id` BIGINT(20) NOT NULL DEFAULT 0 COMMENT '关联支付订单号',
  `way_code` VARCHAR(20) NOT NULL COMMENT '支付方式代码',
  `way_name` VARCHAR(20) NOT NULL COMMENT '支付方式名称',
  `way_type` VARCHAR(20) NOT NULL COMMENT '支付类型: WECHAT-微信, ALIPAY-支付宝, YSFPAY-云闪付, UNIONPAY-银联, OTHER-其他',
  `pay_amount` BIGINT(20) NOT NULL DEFAULT 0 COMMENT '支付金额,单位分',
  `entry_amount` BIGINT(20) NOT NULL DEFAULT 0 COMMENT '入账金额,单位分',
  `give_amount` BIGINT(20) NOT NULL DEFAULT 0 COMMENT '赠送金额,单位分',
  `after_balance` BIGINT(20) NOT NULL DEFAULT 0 COMMENT '账户余额,单位分',
  `avatar_url` VARCHAR(128) DEFAULT NULL COMMENT '头像地址',
  `remark` VARCHAR(128) COMMENT '充值备注',
  `state` TINYINT(6) NOT NULL DEFAULT 0 COMMENT '充值状态: 0-初始化, 1-充值中, 2-充值成功, 3-充值失败',
  `notify_url` VARCHAR(128) NOT NULL DEFAULT '' COMMENT '异步通知地址',
  `return_url` VARCHAR(128) DEFAULT '' COMMENT '页面跳转地址',
  `expired_time` DATETIME DEFAULT NULL COMMENT '订单失效时间',
  `success_time` DATETIME DEFAULT NULL COMMENT '订单支付成功时间',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`recharge_record_id`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COMMENT='商户会员充值记录表';

-- 商户充值规则表
DROP TABLE IF EXISTS t_mbr_recharge_rule;
CREATE TABLE `t_mbr_recharge_rule` (
  `rule_id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '规则ID',
  `mch_no` VARCHAR(64) NOT NULL COMMENT '商户号',
  `recharge_amount` BIGINT(20) NOT NULL DEFAULT 0 COMMENT '充值金额,单位分',
  `give_amount` BIGINT(20) NOT NULL DEFAULT 0 COMMENT '赠送金额,单位分',
  `sort` BIGINT(20) NOT NULL DEFAULT 0 COMMENT '排序',
  `remark` VARCHAR(128) COMMENT '备注',
  `state` TINYINT(6) NOT NULL DEFAULT 1 COMMENT '状态: 0-停用, 1-正常',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`rule_id`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 AUTO_INCREMENT=1001 COMMENT='商户充值规则表';

-- 会员配置
INSERT INTO `t_sys_config` VALUES ('mbrMaxBalance', '会员最大储值余额(元), 0表示依据系统配置', '会员最大储值余额(元), 0表示依据系统配置', 'memberConfig', '会员配置', '1', 'text', 'MCH', 'M0000000000', 0, '2023-08-10 16:30:00');
INSERT INTO `t_sys_config` VALUES ('memberCustomAmountState', '充值自定义金额', '充值自定义金额', 'memberConfig', '会员配置', '1', 'radio', 'MCH', 'M0000000000', 0, '2023-08-10 16:30:00');
INSERT INTO `t_sys_config` VALUES ('memberModelState', '会员模块状态开关', '会员模块状态开关', 'memberConfig', '会员配置', '1', 'radio', 'MCH', 'M0000000000', 0, '2023-08-10 16:30:00');
INSERT INTO `t_sys_config` VALUES ('memberPayState', '会员支付开关', '会员支付开关', 'memberConfig', '会员配置', '1', 'radio', 'MCH', 'M0000000000', 0, '2023-08-10 16:30:00');

-- 会员权限
INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_MEMBER', '会员中心', 'team', '', 'RouteView', 'ML', 0, 1, 'ROOT', 15, 'MCH', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES ('ENT_MEMBER_CONFIG', '会员配置', 'setting', '/member/memberConfig', 'MemberConfigPage', 'ML', 0, 1, 'ENT_MCH_MEMBER', 5, 'MCH', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES ('ENT_MEMBER', '会员管理', 'user', '/member/memberInfo', 'MemberPage', 'ML', 0, 1, 'ENT_MCH_MEMBER', 10, 'MCH', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_MEMBER_LIST', '页面：列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MEMBER', 0, 'MCH', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_MEMBER_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MEMBER', 0, 'MCH', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_MEMBER_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MEMBER', 0, 'MCH', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_MEMBER_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MEMBER', 0, 'MCH', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_MEMBER_MANUAL', '按钮：调账', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MEMBER', 0, 'MCH', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES ('ENT_MEMBER_RECHARGE_RULE', '充值规则', 'profile', '/member/rechargeRule', 'RechargeRulePage', 'ML', 0, 1, 'ENT_MCH_MEMBER', 20, 'MCH', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_MEMBER_RECHARGE_LIST', '页面：列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MEMBER_RECHARGE_RULE', 0, 'MCH', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_MEMBER_RECHARGE_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MEMBER_RECHARGE_RULE', 0, 'MCH', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_MEMBER_RECHARGE_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MEMBER_RECHARGE_RULE', 0, 'MCH', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_MEMBER_RECHARGE_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MEMBER_RECHARGE_RULE', 0, 'MCH', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES ('ENT_MEMBER_ACCOUNT_TRADE', '账户流水', 'exception', '/member/account', 'MemberAccountPage', 'ML', 0, 1, 'ENT_MCH_MEMBER', 30, 'MCH', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_MEMBER_ACCOUNT_TRADE_LIST', '页面：列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MEMBER_ACCOUNT_TRADE', 0, 'MCH', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_MEMBER_RECHARGE_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MEMBER_ACCOUNT_TRADE', 0, 'MCH', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES ('ENT_MEMBER_RECHARGE_RECORD', '充值记录', 'transaction', '/member/recharge', 'MemberRechargePage', 'ML', 0, 1, 'ENT_MCH_MEMBER', 40, 'MCH', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_MEMBER_RECHARGE_RECORD_LIST', '页面：列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MEMBER_RECHARGE_RECORD', 0, 'MCH', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES ('ENT_MEMBER_RECHARGE_RECORD_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MEMBER_RECHARGE_RECORD', 0, 'MCH', NOW(), NOW());

