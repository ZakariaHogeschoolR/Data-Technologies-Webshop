import { useState, useEffect } from 'react';

type UseFetchProps = {
    url: string;
}

export function useFetch<T>({ url }: UseFetchProps) {
    const [data, setData] = useState<T>();
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState("");

    useEffect(() => {
        async function getData() {
            setIsLoading(true);
            try {
                const token = localStorage.getItem("token");

                const headers: HeadersInit = {
                    "Content-Type": "application/json",
                    "Accept": "application/json",
                };

                if (token) {
                    headers["Authorization"] = `Bearer ${token}`;
                }

                const response = await fetch(url, {
                    headers: headers,
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

    return { data, isLoading, error };
}