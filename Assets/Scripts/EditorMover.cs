using UnityEngine;

namespace DefaultNamespace
{
    [RequireComponent(typeof(PositionSaver))]
    public class EditorMover : MonoBehaviour
    {
        private PositionSaver _save;
        private float _currentDelay;

        // todo comment: Что произойдёт, если _delay > _duration?
        // Таймер закончится раньше второго замера —
        // в Records останется одна точка, и ReplayMover не сможет интерполировать движение.

        [Range(0.2f, 1.0f)] // ограничение _delay от 0.2 до 1.0
        [SerializeField] private float _delay = 0.5f;

        [Min(0.2f)] // _duration не может быть меньше 0.2
        [SerializeField] private float _duration = 5f;

        private void Start()
        {
            // todo comment: Почему этот поиск производится здесь, а не в начале метода Update?
            // Потому что GetComponent выполняется один раз,
            // а не каждый кадр, и ссылка готова до первого Update.

            _save = GetComponent<PositionSaver>();
            _save.Records.Clear();

            // Гарантия корректности: _duration должен быть больше _delay.
            // Если это не так — делаем _duration в 5 раз больше _delay,
            // чтобы за время записи успело сработать несколько замеров.

            if (_duration <= _delay)
                _duration = _delay * 5f;
        }

        private void Update()
        {
            _duration -= Time.deltaTime;
            if (_duration <= 0f)
            {
                enabled = false;
                Debug.Log($"<b>{name}</b> finished", this);
                return;
            }

            // todo comment: Почему не написать (_delay -= Time.deltaTime;) по аналогии с полем _duration?
            // Потому что нельзя уменьшать сам _delay, ведь это эталонный интервал между замерами.
            // Рабочий счётчик _currentDelay расходует время и сбрасывается до _delay.

            _currentDelay -= Time.deltaTime;
            if (_currentDelay <= 0f)
            {
                _currentDelay = _delay;
                _save.Records.Add(new PositionSaver.Data
                {
                    Position = transform.position,

                    // todo comment: Для чего сохраняется значение игрового времени?
                    // Время необходимо ReplayMover для интерполяции, тогда по меткам понятно,
                    // когда записана позиция, и можно плавно двигаться в темпе записи.

                    Time = Time.time,
                });
            }
        }
    }
}