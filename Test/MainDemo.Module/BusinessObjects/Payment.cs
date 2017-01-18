using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System;

namespace MainDemo.Module.BusinessObjects {
    [DefaultClassOptions]
    public class Payment : BaseObject {
        private double rate;
        private double hours;

        public Payment(Session session)
            : base(session) {
        }
        [PersistentAlias("Rate * Hours")]
        public double Amount {
            get {
                object tempObject = EvaluateAlias("Amount");
                if(tempObject != null) {
                    return (double)tempObject;
                }
                else {
                    return 0;
                }
            }
        }
        public double Rate {
            get {
                return rate;
            }
            set {
                if(SetPropertyValue("Rate", ref rate, value))
                    OnChanged("Amount");
            }
        }
        public double Hours {
            get {
                return hours;
            }
            set {
                if(SetPropertyValue("Hours", ref hours, value))
                    OnChanged("Amount");
            }
        }
    }
}
