namespace SmtProject.Utils {
	public abstract class LazySingleton<T> where T : LazySingleton<T>, new() {
		static T _instance;
		public static T Instance {
			get {
				if ( _instance == null ) {
					_instance = new T();
					_instance.Init();
				}
				return _instance;
			}
		}

		protected virtual void Init() { }
	}
}
