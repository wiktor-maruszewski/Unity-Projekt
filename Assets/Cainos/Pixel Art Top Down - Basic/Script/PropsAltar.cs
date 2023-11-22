using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//when something get into the alta, make the runes glow
namespace Cainos.PixelArtTopDown_Basic
{

    public class PropsAltar : MonoBehaviour
    {
        public List<SpriteRenderer> runes;
        public float lerpSpeed;
        private float timeNextChange = 0f;

        private Color curColor = new Color(1, 1, 1, 1);
        private Color targetColor = new Color(0f, 0.15f, 1f, 1);

        private void Update()
        {
            if (Time.time >= timeNextChange)
            {
                targetColor = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f), 1);
                timeNextChange = Time.time + Random.Range(0, 5f);
            }
            
            curColor = Color.Lerp(curColor, targetColor, lerpSpeed * Time.deltaTime);

            foreach (var r in runes)
            {
                r.color = curColor;
            }
        }

        public List<Vector2> getRunePositions()
        {
            List<Vector2> positions = new();
            foreach (var rune in runes)
            {
                positions.Add(rune.transform.position);
            }
            return positions;
        }
    }
}
