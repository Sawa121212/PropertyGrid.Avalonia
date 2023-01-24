namespace PropertyGrid.Base
{
    public sealed class TargetPropertyType
    {
        private bool _sealed;
        private Type _type;

        public Type Type
        {
            get => _type;
            set
            {
                if (_sealed)
                    throw new InvalidOperationException(
                        string.Format(
                            "{0}.Type property cannot be modified once the instance is used",
                            typeof(TargetPropertyType)));

                _type = value;
            }
        }

        internal void Seal()
        {
            if (_type == null)
                throw new InvalidOperationException(
                    string.Format("{0}.Type property must be initialized", typeof(TargetPropertyType)));

            _sealed = true;
        }
    }
}