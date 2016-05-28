using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab_Conveyer
{
    public class FacilityNetwork
    {
        public float speed;
        public float swit;
        public float sum;
        public float max;

        private List<int> data = new List<int>();
        public List<float> res = new List<float>();

        private List<int> whole_tact = new List<int>();
        private List<int> whole_tact1 = new List<int>();
        private List<float> rest_mean = new List<float>();
        private List<float> rest_mean1 = new List<float>();

        public List<int> dur_index = new List<int>();
        public List<float> dur_value = new List<float>();

        private List<int> range = new List<int>();

        private List<bool> endcond = new List<bool>();

        public FacilityNetwork(float _speed, List<int> outerdata)
        {
            data = outerdata;
            speed = _speed;

            for (var c = 0; c < data.Count; c++)
                range.Add(c);

            foreach (var z in range)
            {
                if (data[z] <= speed) {
                    whole_tact.Add(data[z] / (int)speed);
                    rest_mean.Add(1);
                }
                else
                {
                    whole_tact.Add(data[z] / (int)speed);
                    if (data[z] % (int)speed != 0)
                        rest_mean.Add(1);
                    else
                        rest_mean.Add(0);
                }
            }
            rest_mean1 = new List<float>(rest_mean);
            whole_tact1 = new List<int>(whole_tact);
            for (int i = 0; i < data.Count; i++)
                endcond.Add(false);


            Schedule();
            CountTask();
        }


        public void Schedule()
        {
            var endschedule = false;
            while (!endschedule)
            {
                foreach (var x in range)
                {
                    if (whole_tact[x] > 0)
                    {
                        whole_tact[x]--;
                        dur_index.Add(x);
                        dur_value.Add(1);
                        continue;
                    }
                    if (rest_mean[x] != 0)
                    {
                        dur_index.Add(x);
                        dur_value.Add(1);
                        rest_mean[x] = 0;
                    }
                    endcond[x] = true;
                }

                var quorum = endcond.Count(f => f);

                if (quorum == endcond.Count)
                    endschedule = true;
            }

            //dur_index.RemoveAt(dur_index.Count - 1);
            //dur_value.RemoveAt(dur_value.Count - 1);
        }

        public void CountTask()
        {
            foreach (var task in range)
            {

                int _pos = 0;
                float occur = 0;
                float cmp = 0;

                res.Add(0);

                if (rest_mean1[task] != 0)
                    cmp = whole_tact1[task] + 1;
                else
                    cmp = whole_tact1[task];

                while (occur != cmp)
                {
                    if (dur_index[_pos] == task)
                        occur++;
                    res[task] += dur_value[_pos];
                    _pos++;
                }
            }
            int iter = 1;
            foreach (var i in res)
            {
                sum += i;
                iter++;
            }

            max = data.Max();

        }
    }
}
