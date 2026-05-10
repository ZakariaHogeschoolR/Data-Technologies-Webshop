import { useState, useEffect } from 'react';

type UseFetchProps = {
    url: string;
}

export function useFetchSecond<T>({ url }: UseFetchProps) {
    const [data2, setData] = useState<T>();
    const [isLoading2, setIsLoading] = useState(false);
    const [error2, setError] = useState("");

    useEffect(() => {
        async function getData() {
            setIsLoading(true);
            try {
                const response = await fetch(url, {
                    headers: {
                        "Content-Type": "application/json",
                        "Accept": "application/json",
                    },
                });

                if (!response.ok) {
                    const text = await response.text().catch(() => 'Unknown error');
                    throw new Error(`HTTP ${response.status}: ${text}`);
                }

                const responseData = await response.json();
                setData(responseData);
            } catch (err) {
                if (err instanceof Error) {
                    setError(err.message);
                } else {
                    setError("Something went wrong");
                }
            } finally {
                setIsLoading(false);
            }
        }

        getData();
    }, [url]);

    return { data2, isLoading2, error2 };
}