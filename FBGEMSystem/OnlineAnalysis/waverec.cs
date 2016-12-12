﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBGEMSystem
{
    public class waverec
    {
        /*
         * 函数说明：根据小波名称，确定小波基参数
         * 输入：
         *      string wavename, 小波名称
         *      ref int dwt_mode, 小波基
         *      ref int len 小波基
         * 输出：
         *  无
         */
        private void dwt_mode_select(string wavename, ref int dwt_mode, ref int len)
        {
            int l = wavename.Length;

            string wave = wavename.Substring(0, l - 1);
            switch (wave)
            {
                case "db": dwt_mode = 1; break;
                case "coif": dwt_mode = 2; break;
                case "sym": dwt_mode = 3; break;
                default: break;
            }
            len = int.Parse(wavename.Substring(l - 1, 1));
        }
        /*
         * 函数说明：小波分解函数
         * 输入：
         *              double [] data_C, 小波分解系数矩阵
         *               int [] data_L, 小波分解系数长度矩阵
         *                string wavename,  小波重构名称
         *                ref double [] result,  重构后序列数组
         *                int level  重构至第几层
         * 输出：
         *  无
         */
        public void wrec(double [] data_C, 
                         int [] data_L,
                         string wavename, 
                         ref double [] result, 
                         int level)
        {
            double[] Lo_R = new double[0];
            double[] Hi_R = new double[0];
            int dwt_mode = 0;
            int len_n = 0;
            dwt_mode_select(wavename, ref dwt_mode, ref len_n);

            string sig_mode = "per1";

            switch (dwt_mode)
            {
                //db
                case 1:
                    {
                        Lo_R = new double[2 * len_n];
                        Hi_R = new double[2 * len_n];
                        Daubechies_coefficient(len_n, ref Lo_R, ref Hi_R);
                    }
                    break;
                //coif
                case 2:
                    {
                        Lo_R = new double[6 * len_n];
                        Hi_R = new double[6 * len_n];
                        Coiflets_coefficient(len_n, ref Lo_R, ref Hi_R);
                    }
                    break;
                //sym
                case 3:
                    {
                        Lo_R = new double[2 * len_n];
                        Hi_R = new double[2 * len_n];
                        Symlets_coefficient(len_n, ref Lo_R, ref Hi_R);
                    }
                    break;
            }

            result = appcoef(data_C, data_L, Lo_R, Hi_R, level, sig_mode);
        }

        /*
        *  函数说明：根据N选择dbN的重构高通、低通滤波系数
        *  输入：
        *       int N ： 选择dbN
        *       ref double[] Lo_Rref ：重构低通滤波系数
        *       double[] Hi_R ： 重构高通滤波系数
        *  输出：
        *       无
        */
        private static void Daubechies_coefficient(int N, ref double[] Lo_R, ref double[] Hi_R)
        {
            Lo_R = new double[2 * N];
            Hi_R = new double[2 * N];
            switch (N)
            {
                case 1: Lo_R = new double[] { 0.707106781186548, 0.707106781186548 }; Hi_R = new double[] { 0.707106781186548, -0.707106781186548 }; break;
                case 2: Lo_R = new double[] { 0.482962913144690, 0.836516303737469, 0.224143868041857, -0.129409522550921 }; Hi_R = new double[] { -0.129409522550921, -0.224143868041857, 0.836516303737469, -0.482962913144690 }; break;
                case 3: Lo_R = new double[] { 0.332670552950957, 0.806891509313339, 0.459877502119331, -0.135011020010391, -0.085441273882241, 0.035226291882101 }; Hi_R = new double[] { 0.035226291882101, 0.085441273882241, -0.135011020010391, -0.459877502119331, 0.806891509313339, -0.332670552950957 }; break;
                case 4: Lo_R = new double[] { 0.230377813308855, 0.714846570552542, 0.630880767929590, -0.027983769416984, -0.187034811718881, 0.030841381835987, 0.032883011666983, -0.010597401784997 }; Hi_R = new double[] { -0.010597401784997, -0.032883011666983, 0.030841381835987, 0.187034811718881, -0.027983769416984, -0.630880767929590, 0.714846570552542, -0.230377813308855 }; break;
                case 5: Lo_R = new double[] { 0.160102397974125, 0.603829269797473, 0.724308528438574, 0.138428145901103, -0.242294887066190, -0.032244869585030, 0.077571493840065, -0.006241490213012, -0.012580751999016, 0.003335725285002 }; Hi_R = new double[] { 0.003335725285002, 0.012580751999016, -0.006241490213012, -0.077571493840065, -0.032244869585030, 0.242294887066190, 0.138428145901103, -0.724308528438574, 0.603829269797473, -0.160102397974125 }; break;
                case 6: Lo_R = new double[] { 0.111540743350080, 0.494623890398385, 0.751133908021578, 0.315250351709243, -0.226264693965169, -0.129766867567096, 0.097501605587079, 0.027522865530016, -0.031582039318031, 0.000553842200994, 0.004777257511011, -0.001077301084996 }; Hi_R = new double[] { -0.001077301084996, -0.004777257511011, 0.000553842200994, 0.031582039318031, 0.027522865530016, -0.097501605587079, -0.129766867567096, 0.226264693965169, 0.315250351709243, -0.751133908021578, 0.494623890398385, -0.111540743350080 }; break;
                case 7: Lo_R = new double[] { 0.077852054085062, 0.396539319482306, 0.729132090846555, 0.469782287405359, -0.143906003929106, -0.224036184994166, 0.071309219267050, 0.080612609151066, -0.038029936935035, -0.016574541631016, 0.012550998556014, 0.000429577973005, -0.001801640704000, 0.000353713800001 }; Hi_R = new double[] { 0.000353713800001, 0.001801640704000, 0.000429577973005, -0.012550998556014, -0.016574541631016, 0.038029936935035, 0.080612609151066, -0.071309219267050, -0.224036184994166, 0.143906003929106, 0.469782287405359, -0.729132090846555, 0.396539319482306, -0.077852054085062 }; break;
                case 8: Lo_R = new double[] { 0.054415842243082, 0.312871590914466, 0.675630736298013, 0.585354683654869, -0.015829105256024, -0.284015542962428, 0.000472484573998, 0.128747426620186, -0.017369301002022, -0.044088253931065, 0.013981027917016, 0.008746094047016, -0.004870352993011, -0.000391740372996, 0.000675449405999, -0.000117476784002 }; Hi_R = new double[] { -0.000117476784002, -0.000675449405999, -0.000391740372996, 0.004870352993011, 0.008746094047016, -0.013981027917016, -0.044088253931065, 0.017369301002022, 0.128747426620186, -0.000472484573998, -0.284015542962428, 0.015829105256024, 0.585354683654869, -0.675630736298013, 0.312871590914466, -0.054415842243082 }; break;
                case 9: Lo_R = new double[] { 0.038077947363167, 0.243834674637667, 0.604823123676779, 0.657288078036639, 0.133197385822089, -0.293273783272587, -0.096840783220879, 0.148540749334760, 0.030725681478323, -0.067632829059524, 0.000250947114992, 0.022361662123515, -0.004723204757895, -0.004281503681905, 0.001847646882961, 0.000230385763995, -0.000251963188998, 0.000039347319995 }; Hi_R = new double[] { 0.000039347319995, 0.000251963188998, 0.000230385763995, -0.001847646882961, -0.004281503681905, 0.004723204757895, 0.022361662123515, -0.000250947114992, -0.067632829059524, -0.030725681478323, 0.148540749334760, 0.096840783220879, -0.293273783272587, -0.133197385822089, 0.657288078036639, -0.604823123676779, 0.243834674637667, -0.038077947363167 }; break;
                case 10: Lo_R = new double[] { 0.026670057900951, 0.188176800077621, 0.527201188930920, 0.688459039452592, 0.281172343660426, -0.249846424326489, -0.195946274376597, 0.127369340335743, 0.093057364603807, -0.071394147165861, -0.029457536821946, 0.033212674058933, 0.003606553566988, -0.010733175482980, 0.001395351746994, 0.001992405294991, -0.000685856695005, -0.000116466854994, 0.000093588670001, -0.000013264203002 }; Hi_R = new double[] { -0.000013264203002, -0.000093588670001, -0.000116466854994, 0.000685856695005, 0.001992405294991, -0.001395351746994, -0.010733175482980, -0.003606553566988, 0.033212674058933, 0.029457536821946, -0.071394147165861, -0.093057364603807, 0.127369340335743, 0.195946274376597, -0.249846424326489, -0.281172343660426, 0.688459039452592, -0.527201188930920, 0.188176800077621, -0.026670057900951 }; break;
                default:break;
            }
        }

        /*
         *  函数说明：根据N选择Coif的重构高通、低通滤波系数
         *  输入：
         *       int N ： 选择小波基
         *       ref double[] Lo_Rref ：重构低通滤波系数
         *       double[] Hi_R ： 重构高通滤波系数
         *  输出：
         *       无
         */
        private static void Coiflets_coefficient(int N, ref double[] Lo_R, ref double[] Hi_R)
        {
            Lo_R = new double[6 * N];
            Hi_R = new double[6 * N];
            switch (N)
            {
                case 1: Lo_R = new double[] { -0.072732619512854, 0.337897662457809, 0.852572020212255, 0.384864846864203, -0.072732619512854, -0.015655728135465 }; Hi_R = new double[] { -0.015655728135465, 0.072732619512854, 0.384864846864203, -0.852572020212255, 0.337897662457809, 0.072732619512854 }; break;
                case 2: Lo_R = new double[] { 0.016387336463522, -0.041464936781759, -0.067372554721963, 0.386110066821162, 0.812723635445542, 0.417005184421693, -0.076488599078306, -0.059434418646457, 0.023680171946334, 0.005611434819394, -0.001823208870703, -0.000720549445365 }; Hi_R = new double[] { -0.000720549445365, 0.001823208870703, 0.005611434819394, -0.023680171946334, -0.059434418646457, 0.076488599078306, 0.417005184421693, -0.812723635445542, 0.386110066821162, 0.067372554721963, -0.041464936781759, -0.016387336463522 }; break;
                case 3: Lo_R = new double[] { -0.003793512864491, 0.007782596427325, 0.023452696141836, -0.065771911281856, -0.061123390002673, 0.405176902409617, 0.793777222625621, 0.428483476377619, -0.071799821619312, -0.082301927106886, 0.034555027573062, 0.015880544863616, -0.009007976136662, -0.002574517688750, 0.001117518770891, 0.000466216960113, -0.000070983303138, -0.000034599772836 }; Hi_R = new double[] { -0.000034599772836, 0.000070983303138, 0.000466216960113, -0.001117518770891, -0.002574517688750, 0.009007976136662, 0.015880544863616, -0.034555027573062, -0.082301927106886, 0.071799821619312, 0.428483476377619, -0.793777222625621, 0.405176902409617, 0.061123390002673, -0.065771911281856, -0.023452696141836, 0.007782596427325, 0.003793512864491 }; break;
                case 4: Lo_R = new double[] { 0.000892313668582, -0.001629492012602, -0.007346166327642, 0.016068943964776, 0.026682300156053, -0.081266699680879, -0.056077313316755, 0.415308407030430, 0.782238930920499, 0.434386056491469, -0.066627474263425, -0.096220442033988, 0.039334427123337, 0.025082261844864, -0.015211731527946, -0.005658286686611, 0.003751436157278, 0.001266561929299, -0.000589020756244, -0.000259974552488, 0.000062339034461, 0.000031229875865, -0.000003259680237, -0.000001784985003 }; Hi_R = new double[] { -0.000001784985003, 0.000003259680237, 0.000031229875865, -0.000062339034461, -0.000259974552488, 0.000589020756244, 0.001266561929299, -0.003751436157278, -0.005658286686611, 0.015211731527946, 0.025082261844864, -0.039334427123337, -0.096220442033988, 0.066627474263425, 0.434386056491469, -0.782238930920499, 0.415308407030430, 0.056077313316755, -0.081266699680879, -0.026682300156053, 0.016068943964776, 0.007346166327642, -0.001629492012602, -0.000892313668582 }; break;
                case 5: Lo_R = new double[] { -0.000212080839804, 0.000358589687896, 0.002178236358109, -0.004159358781386, -0.010131117519850, 0.023408156785839, 0.028168028970936, -0.091920010559696, -0.052043163176244, 0.421566206690851, 0.774289603652956, 0.437991626171837, -0.062035963962904, -0.105574208703339, 0.041289208750182, 0.032683574267112, -0.019761778942573, -0.009164231162482, 0.006764185448053, 0.002433373212658, -0.001662863702013, -0.000638131343045, 0.000302259581813, 0.000140541149702, -0.000041340432273, -0.000021315026810, 0.000003734655175, 0.000002063761851, -0.000000167442886, -0.000000095176573 }; Hi_R = new double[] { -0.000000095176573, 0.000000167442886, 0.000002063761851, -0.000003734655175, -0.000021315026810, 0.000041340432273, 0.000140541149702, -0.000302259581813, -0.000638131343045, 0.001662863702013, 0.002433373212658, -0.006764185448053, -0.009164231162482, 0.019761778942573, 0.032683574267112, -0.041289208750182, -0.105574208703339, 0.062035963962904, 0.437991626171837, -0.774289603652956, 0.421566206690851, 0.052043163176244, -0.091920010559696, -0.028168028970936, 0.023408156785839, 0.010131117519850, -0.004159358781386, -0.002178236358109, 0.000358589687896, 0.000212080839804 }; break;
                default: break;
            }

        }

        /*
         *  函数说明：根据N选择sym的重构高通、低通滤波系数
         *  输入：
         *       int N ： 选择小波基
         *       ref double[] Lo_Rref ：重构低通滤波系数
         *       double[] Hi_R ： 重构高通滤波系数
         *  输出：
         *       无
         */
        private static void Symlets_coefficient(int N, ref double[] Lo_R, ref double[] Hi_R)
        {
            double[] Lo_D1 = new double[2 * N];
            double[] Hi_D1 = new double[2 * N];
            switch (N)
            {
                case 2: Lo_R = new double[] { 0.482962913144690, 0.836516303737469, 0.224143868041857, -0.129409522550921 }; Hi_R = new double[] { -0.129409522550921, -0.224143868041857, 0.836516303737469, -0.482962913144690 }; break;
                case 3: Lo_R = new double[] { 0.332670552950957, 0.806891509313339, 0.459877502119331, -0.135011020010391, -0.085441273882241, 0.035226291882101 }; Hi_R = new double[] { 0.035226291882101, 0.085441273882241, -0.135011020010391, -0.459877502119331, 0.806891509313339, -0.332670552950957 }; break;
                case 4: Lo_R = new double[] { 0.032223100604043, -0.012603967262038, -0.099219543576847, 0.297857795605277, 0.803738751805916, 0.497618667632015, -0.029635527645999, -0.075765714789273 }; Hi_R = new double[] { -0.075765714789273, 0.029635527645999, 0.497618667632015, -0.803738751805916, 0.297857795605277, 0.099219543576847, -0.012603967262038, -0.032223100604043 }; break;
                case 5: Lo_R = new double[] { 0.019538882735287, -0.021101834024759, -0.175328089908450, 0.016602105764522, 0.633978963458212, 0.723407690402421, 0.199397533977394, -0.039134249302383, 0.029519490925775, 0.027333068345078 }; Hi_R = new double[] { 0.027333068345078, -0.029519490925775, -0.039134249302383, -0.199397533977394, 0.723407690402421, -0.633978963458212, 0.016602105764522, 0.175328089908450, -0.021101834024759, -0.019538882735287 }; break;
                case 6: Lo_R = new double[] { -0.007800708325034, 0.001767711864243, 0.044724901770666, -0.021060292512301, -0.072637522786463, 0.337929421727622, 0.787641141030194, 0.491055941926747, -0.048311742585633, -0.117990111148191, 0.003490712084217, 0.015404109327027 }; Hi_R = new double[] { 0.015404109327027, -0.003490712084217, -0.117990111148191, 0.048311742585633, 0.491055941926747, -0.787641141030194, 0.337929421727622, 0.072637522786463, -0.021060292512301, -0.044724901770666, 0.001767711864243, 0.007800708325034 }; break;
                case 7: Lo_R = new double[] { 0.010268176708511, 0.004010244871534, -0.107808237703818, -0.140047240442962, 0.288629631751515, 0.767764317003164, 0.536101917091763, 0.017441255086856, -0.049552834937127, 0.067892693501373, 0.030515513165964, -0.012636303403252, -0.001047384888683, 0.002681814568258 }; Hi_R = new double[] { 0.002681814568258, 0.001047384888683, -0.012636303403252, -0.030515513165964, 0.067892693501373, 0.049552834937127, 0.017441255086856, -0.536101917091763, 0.767764317003164, -0.288629631751515, -0.140047240442962, 0.107808237703818, 0.004010244871534, -0.010268176708511 }; break;
                case 8: Lo_R = new double[] { 0.001889950332759, -0.000302920514721, -0.014952258337048, 0.003808752013891, 0.049137179673608, -0.027219029917056, -0.051945838107709, 0.364441894835331, 0.777185751700524, 0.481359651258372, -0.061273359067659, -0.143294238350810, 0.007607487324918, 0.031695087811493, -0.000542132331791, -0.003382415951006 }; Hi_R = new double[] { -0.003382415951006, 0.000542132331791, 0.031695087811493, -0.007607487324918, -0.143294238350810, 0.061273359067659, 0.481359651258372, -0.777185751700524, 0.364441894835331, 0.051945838107709, -0.027219029917056, -0.049137179673608, 0.003808752013891, 0.014952258337048, -0.000302920514721, -0.001889950332759 }; break;
                case 9: Lo_D1 = new double[] { 0.001069490032909, -0.000473154498680, -0.010264064027633, 0.008859267493400, 0.062077789302886, -0.018233770779395, -0.191550831297285, 0.035272488035272, 0.617338449140936, 0.717897082764413, 0.238760914607304, -0.054568958430835, 0.000583462746125, 0.030224878858275, -0.011528210207679, -0.013271967781817, 0.000619780888986, 0.001400915525915 }; Hi_R = new double[] { 0.001400915525915, -0.000619780888986, -0.013271967781817, 0.011528210207679, 0.030224878858275, -0.000583462746125, -0.054568958430835, -0.238760914607304, 0.717897082764413, -0.617338449140936, 0.035272488035272, 0.191550831297285, -0.018233770779395, -0.062077789302886, 0.008859267493400, 0.010264064027633, -0.000473154498680, -0.001069490032909 }; break;
                case 10: Lo_D1 = new double[] { -0.000459329421005, 0.000057036083618, 0.004593173585312, -0.000804358932016, -0.020354939812311, 0.005764912033581, 0.049994972077375, -0.031990056882431, -0.035536740473826, 0.383826761067071, 0.769510037021100, 0.471690666938450, -0.070880535783227, -0.159494278884910, 0.011609893903711, 0.045927239231092, -0.001465382581304, -0.008641299277022, 0.000095632670723, 0.000770159809114 }; Hi_R = new double[] { 0.000770159809114, -0.000095632670723, -0.008641299277022, 0.001465382581304, 0.045927239231092, -0.011609893903711, -0.159494278884910, 0.070880535783227, 0.471690666938450, -0.769510037021100, 0.383826761067071, 0.035536740473826, -0.031990056882431, -0.049994972077375, 0.005764912033581, 0.020354939812311, -0.000804358932016, -0.004593173585312, 0.000057036083618, 0.000459329421005 }; break;
                default: break;
            }
        }

        /*
         * 函数说明：获取第N层的近似系数
         *      输入：
         *          double[] data_C, 小波分解系数矩阵
         *          int[] data_L, 小波分解各层系数长度数组
         *          double[] Lo_R, 重构低通滤波器
         *          double[] Hi_R, 重构高通滤波器
         *          int N, 第N层的近似系数
         *          string signal_mode 重构模式，未实现
         *      输出：
         *          返回第N层近似系数矩阵
         */
        private double[] appcoef(double[] data_C, 
                                        int[] data_L, 
                                        double[] Lo_R, 
                                        double[] Hi_R, 
                                        int N, 
                                        string signal_mode)
        {
            int len = data_L[0]; //最大层的点数
            double[] temp = new double[len];
            int rmax = data_L.Length;
            int nmax = rmax - 2; //获取最大的层数
            for (int i = 0; i < len; i++)
            {
                temp[i] = data_C[i];
            }
            double[] temp_d = new double[0];

            for (int i = nmax; i >= N + 1; i--)
            {
                temp_d = detcoef(data_C, data_L, i);
                temp = idwt(temp, temp_d, Lo_R, Hi_R, signal_mode, data_L[rmax - i]);
            }
            return temp;
        }

        /*
         * 函数说明：提取细节系数
         *   输入：
         *      double[] data_C ：小波分解系数数组
         *      int[] data_L ： 小波分解各分量长度数组
         *      int N ：第N层
         *   输出：
         *      返回第N层细节系数矩阵
         */
        public double[] detcoef(double[] data_C, int[] data_L, int N)
        {
            int[] temp_1 = new int[data_L.Length];
            for (int i = 0; i < temp_1.Length; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    temp_1[i] += data_L[j];
                }
                temp_1[i] += 1;
            }
            int[] first = new int[data_L.Length - 2]; //第N层起始点
            int[] last = new int[data_L.Length - 2];//第N层结束点
            int[] temp_2 = new int[data_L.Length - 2]; //中间变量，保存第N层的点数
            for (int i = 0; i < first.Length; i++)
            {
                first[i] = temp_1[temp_1.Length - 3 - i];
            }
            for (int i = 0; i < temp_2.Length; i++)
            {
                temp_2[i] = data_L[data_L.Length - 2 - i];
            }
            for (int i = 0; i < last.Length; i++)
            {
                last[i] = first[i] + temp_2[i] - 1;
            }
            int data_final_len = last[N - 1] - first[N - 1] + 1;  //根据N，确定返回数组长度
            double[] data_final = new double[data_final_len];

            for (int i = 0; i < data_final_len; i++)
            {
                data_final[i] = data_C[first[N - 1] + i - 1];
            }
            return data_final;
        }

        /*
         * 函数说明：单层小波重建
         * 输入：
         *      double[] data_cA,待重建小波近似系数
         *      double[] data_cD, 待重建小波细节系数
         *      double[] Lo_R,重建低通滤波器
         *      double[] Hi_R, 重建高通滤波器
         *      string signal_mode, 重建模式
         *      int LEN，
         * 输出：
         *      重建的小波系数数组
         */
        private static double[] idwt(double[] data_cA, 
                                     double[] data_cD, 
                                     double[] Lo_R, 
                                     double[] Hi_R, 
                                     string signal_mode, 
                                     int LEN)
        {
            double[] data_cA_extend = new double[0];
            double[] data_cD_extend = new double[0];

            //lf为滤波器的长度
            int lf = Lo_R.Length;
            int len = lf / 2;
            int lx = data_cA.Length;


            //Signal_MODE(ref signal_mode);
            switch (signal_mode)
            {
                case "per":
                    {
                        Upsampling(ref data_cA, ref data_cA_extend, signal_mode);
                        Upsampling(ref data_cD, ref data_cD_extend, signal_mode);
                        data_cA_extend = extend_per(ref data_cA_extend, ref  len);
                        data_cD_extend = extend_per(ref data_cD_extend, ref  len);

                    }
                    break;
                default:
                    {
                        Upsampling(ref data_cA, ref data_cA_extend, signal_mode);
                        Upsampling(ref data_cD, ref data_cD_extend, signal_mode);
                    }
                    break;
            }
            double[] cA_conv = new double[0];
            double[] cD_conv = new double[0];
            double[] cA_final = new double[0];
            double[] cD_final = new double[0];
            cA_conv = Conv(data_cA_extend, Lo_R);
            cD_conv = Conv(data_cD_extend, Hi_R);

            switch (signal_mode)
            {
                case "per":
                    {
                        int ext_len = 2 * lx;
                        if (LEN >= 0 && LEN < ext_len)
                            ext_len = LEN;
                        cA_final = new double[ext_len];
                        cD_final = new double[ext_len];
                        for (int i = 0; i < ext_len; i++)
                        {
                            cA_final[i] = cA_conv[i + lf - 1];
                            cD_final[i] = cD_conv[i + lf - 1];
                        }
                    }
                    break;
                default:
                    {
                        int ext_len = 2 * lx - lf + 2;
                        if (LEN >= 0 && LEN < ext_len)
                            ext_len = LEN;
                        cA_final = wkeep1(cA_conv, ext_len);
                        cD_final = wkeep1(cD_conv, ext_len);
                    }
                    break;
            }
            double[] result = new double[cA_final.Length];
            for (int i = 0; i < cA_final.Length; i++)
            {
                result[i] = cA_final[i] + cD_final[i];
            }
            return result;
        }

        /*
        * 函数说明：信号抽样，2值抽样，隔点取值
        * 输入：
        *      ref double[] data_in ： 序列输入数组
        *      ref double[] data_out： 序列输出数组
        *      string mode ： 抽样模式选择
        * 输出：
        *      无
        */
        private static void Upsampling(ref double[] data_in, ref double[] data_out, string mode)
        {
            int N = data_in.Length;
            switch (mode)
            {
                case "per":
                    {
                        data_out = new double[2 * N];
                    }
                    break;
                default:
                    {
                        data_out = new double[2 * N - 1];
                    }
                    break;
            }
            for (int i = 0; i < data_out.Length; i++)
            {
                if (i % 2 == 0)
                    data_out[i] = data_in[i / 2];
                else
                    data_out[i] = 0;
            }
        }

        //信号的周期延拓模式（如果信号长度是奇数，则在信号的右边添一个和最后一个采样值一样的值，然后在进行周期延拓）
        /*
         * 函数说明：对信号进行周期延拓后，取（信号长度+2*len）长度信号，信号组成为：原信号最后len个点 + 原信号 + 原信号初始len个点
         *              若len小于1，直接返回原始信号。若信号长度为奇数，补齐至偶数个点。
         *  输入：
         *      ref double[] SIGNAL ： 原始信号序列
         *      ref int len ： 需要取得len长度
         *  输出：
         *      返回延拓后信号
         */
        private static double[] extend_per(ref double[] SIGNAL, ref int len)
        {
            int len1 = SIGNAL.Length;
            double[] SIGNAL_1;
            if (len1 % 2 != 0)
            {
                SIGNAL_1 = new double[len1 + 1];
                for (int i = 0; i < len1; i++)
                {
                    SIGNAL_1[i] = SIGNAL[i];
                }
                SIGNAL_1[len1] = SIGNAL[len1 - 1];
            }
            else
            {
                SIGNAL_1 = new double[len1];
                SIGNAL_1 = SIGNAL;
            }
            int Len_SIG = SIGNAL_1.Length;

            double[] temp = new double[Len_SIG + len * 2];
            if (len < 1)
                return SIGNAL_1;
            else
            {
                for (int i = 0; i < len; i++)
                {
                    temp[i] = SIGNAL_1[Len_SIG - len + i];
                }
                for (int i = 0; i < Len_SIG; i++)
                {
                    temp[i + len] = SIGNAL_1[i];
                }
                for (int i = 0; i < len; i++)
                {
                    temp[i + len + Len_SIG] = SIGNAL_1[i];
                }
            }
            return temp;
        }

        //信号卷积函数（其类似conv2（x,h,'full')实现的卷积功能）
        /*
         * 函数说明：信号卷积函数，线性卷积
         * 输入：
         *      double[] signal  ： 原始信号
         *      double[] filters ： 滤波器信号
         * 输出：
         *      返回线性卷积的结果数组
         */
        private static double[] Conv(double[] signal, double[] filters)
        {
            int len1 = signal.Length;
            int len2 = filters.Length;
            int len = len1 + len2 - 1;
            double[] result = new double[len];
            for (int i = 0; i < len; i++)
            {
                double flags = 0;
                if (i < len2 - 1)
                {
                    for (int j = 0; j < len1; j++)
                    {
                        if ((i - j) >= 0 && (i - j) < len2)
                            flags += signal[j] * filters[i - j];
                    }
                }
                else
                {
                    for (int j = 0; j < len2; j++)
                    {
                        if ((i - len2 + 1 + j) < len1)
                            flags += signal[i - len2 + 1 + j] * filters[len2 - 1 - j];
                    }
                }
                result[i] = flags;
            }
            return result;
        }

        /*
         * 函数说明：延拓信号格式： 原信号最后len个点 + 原信号 + 原信号开始len，此函数，获取中间的原信号
         *      输入：
         *          double[] data, 延拓信号序列
         *          int ext_len，原信号长度
         *      输出：
         *          返回原信号数组
         */
        private static double[] wkeep1(double[] data, int ext_len)
        {
            double[] temp = new double[ext_len];
            int len = data.Length;
            int first = ((len - ext_len) / 2);
            for (int i = 0; i < ext_len; i++)
            {
                temp[i] = data[i + first];
            }
            return temp;
        }
    }
}
