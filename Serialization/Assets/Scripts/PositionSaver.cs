using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DefaultNamespace
{
    public class PositionSaver : MonoBehaviour
    {
        //todo comment:
        // Задание: сделать структуру Data сериализуемой.
        // Без атрибута [Serializable] JsonUtility не сериализует структуру,
        // и Records при записи в JSON окажется пустым.
        [Serializable]
        public struct Data
        {
            public Vector3 Position;
            public float Time;
        }

        // Задание: сделать поле _json нередактируемым в инспекторе и добавить всплывающую подсказку.
        // ReadOnly — атрибут из задания 2 (поле видно, но изменять нельзя),
        // Tooltip — при наведении курсора на поле в инспекторе появляется подсказка
        // о том, что заполнять его нужно через контекстное меню и команду "Create File".
        // SerializeField нужен, чтобы ссылка на ассет сохранялась между запусками.
        [ReadOnly, Tooltip("Заполните через контекстное меню компонента: команда \"Create File\"")]
        [SerializeField]
        private TextAsset _json;

        // Задание: сделать автосвойство Records сериализуемым и скрытым в инспекторе.
        // Атрибуты с модификатором field: вешаются на скрытое backing-поле свойства:
        // Unity сериализует его, но в инспекторе не показывает.
        // Приватный сеттер сохранён: присваивать извне нельзя, а FromJsonOverwrite
        // запишет значение в backing-поле напрямую.
        [field: SerializeField, HideInInspector]
        public List<Data> Records { get; private set; }

        private void Awake()
        {
            //todo comment: Что будет, если в теле этого условия не сделать выход из метода?
            // Выполнение метода продолжится, и следующая строка
            // JsonUtility.FromJsonOverwrite(_json.text, this) бросит NullReferenceException,
            // потому что _json == null. return прерывает метод,
            // чтобы не работать с нулевым объектом.
            if (_json == null)
            {
                gameObject.SetActive(false);
                Debug.LogError("Please, create TextAsset and add in field _json");
                return;
            }

            JsonUtility.FromJsonOverwrite(_json.text, this);
            //todo comment: Для чего нужна эта проверка (что она позволяет избежать)?
            // FromJsonOverwrite заполнит Records, только если это поле есть в JSON.
            // Если файл пустой или битый — поле останется null, и дальше по коду
            // (например, в OnDrawGizmos) будет NullReferenceException.
            // По сути, проверка гарантирует, что список всегда инициализирован.
            if (Records == null)
                Records = new List<Data>(10);
        }

        private void OnDrawGizmos()
        {
            //todo comment: Зачем нужны эти проверки (что они позволляют избежать)?
            // OnDrawGizmos вызывается редактором постоянно, в том числе до Awake.
            // Records == null — это защита от NullReferenceException при обращении к списку;
            // Records.Count == 0 — это защита от выхода за границы списка (data[0]) при пустом списке.
            if (Records == null || Records.Count == 0) return;
            var data = Records;
            var prev = data[0].Position;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(prev, 0.3f);
            //todo comment: Почему итерация начинается не с нулевого элемента?
            // Потому что нулевой элемент уже отрисован строкой выше (prev = data[0].Position):
            // с i = 0 первая сфера рисовалась бы дважды,
            // а линия шла бы из точки в неё же саму.
            for (int i = 1; i < data.Count; i++)
            {
                var curr = data[i].Position;
                Gizmos.DrawWireSphere(curr, 0.3f);
                Gizmos.DrawLine(prev, curr);
                prev = curr;
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Create File")]
        private void CreateFile()
        {
            //todo comment: Что происходит в этой строке?
            // Создаётся (или перезаписывается, если уже есть) пустой файл Path.txt
            // в папке Assets (Application.dataPath — путь к папке Assets,
            // Path.Combine собирает полный путь).
            // File.Create возвращает FileStream — открытый поток записи в файл.
            var stream = File.Create(Path.Combine(Application.dataPath, "Path.txt"));
            //todo comment: Подумайте для чего нужна эта строка? (а потом проверьте догадку, закомментировав)
            // Поток держит файл открытым: пока он не закрыт, AssetDatabase.Refresh()
            // не сможет импортировать файл, а данные не гарантированно записаны на диск.
            // Dispose закрывает поток и освобождает дескриптор.
            // Если же закомментировать — файл может вовсе не появиться в окне Project,
            // и FindAssets его не найдёт.
            stream.Dispose();
            UnityEditor.AssetDatabase.Refresh();
            //В Unity можно искать объекты по их типу, для этого используется префикс "t:"
            //После нахождения, Юнити возвращает массив гуидов (которые задаются, например, в мета-файлах)
            var guids = UnityEditor.AssetDatabase.FindAssets("t:TextAsset");
            foreach (var guid in guids)
            {
                //Этой командой можно получить путь к ассету через его гуид
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                //Этой командой можно загрузить сам ассет
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                //todo comment: Для чего нужны эти проверки?
                // asset != null — LoadAssetAtPath может вернуть null для части ассетов,
                // без проверки будет NullReferenceException на asset.name.
                // asset.name == "Path" — присваиваем _json именно наш файл,
                // а не первый попавшийся TextAsset в проекте.
                if (asset != null && asset.name == "Path")
                {
                    _json = asset;
                    UnityEditor.EditorUtility.SetDirty(this);
                    UnityEditor.AssetDatabase.SaveAssets();
                    UnityEditor.AssetDatabase.Refresh();
                    //todo comment: Почему мы здесь выходим, а не продолжаем итерироваться?
                    // Потому что нужный нам ассет уже найден — задача решена, дальнейший поиск попросту бессмысленен.
                    // + если в проекте несколько ассетов с именем "Path",
                    // продолжение цикла перезаписало бы _json последним из них
                    // и зря дёргало SaveAssets на каждой итерации.
                    return;
                }
            }
        }

        private void OnDestroy()
        {
            //todo logic...
            // Сериализуем Records (и всё сериализуемое) в JSON-ассет из поля _json —
            // обратная операция к десериализации в Awake.
            // Проверка нужна, чтобы не упасть, если файл так и не создали
            // или Records не инициализирован.
            if (_json == null || Records == null)
                return;

            var path = UnityEditor.AssetDatabase.GetAssetPath(_json);
            File.WriteAllText(path, JsonUtility.ToJson(this));
            UnityEditor.EditorUtility.SetDirty(_json);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }
#endif
    }
}